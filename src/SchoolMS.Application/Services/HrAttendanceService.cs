using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrAttendanceService : IHrAttendanceService
{
    private readonly IRepository<HrDailyAttendance> _attendanceRepo;
    private readonly IRepository<HrFingerprintRecord> _fingerprintRepo;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IRepository<HrLeaveRequest> _leaveRepo;
    private readonly IRepository<HrHoliday> _holidayRepo;
    private readonly IUnitOfWork _unitOfWork;

    public HrAttendanceService(IRepository<HrDailyAttendance> attendanceRepo,
        IRepository<HrFingerprintRecord> fingerprintRepo, IRepository<HrEmployee> employeeRepo,
        IRepository<HrLeaveRequest> leaveRepo, IRepository<HrHoliday> holidayRepo,
        IUnitOfWork unitOfWork)
    {
        _attendanceRepo = attendanceRepo;
        _fingerprintRepo = fingerprintRepo;
        _employeeRepo = employeeRepo;
        _leaveRepo = leaveRepo;
        _holidayRepo = holidayRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<HrDailyAttendanceDto>> GetDailyAttendanceAsync(DateTime date, int? departmentId = null, int? branchId = null, int? employeeId = null)
    {
        var query = _attendanceRepo.Query()
            .Include(a => a.Employee)
            .Include(a => a.WorkShift)
            .Where(a => a.AttendanceDate == date.Date);

        if (departmentId.HasValue)
            query = query.Where(a => a.Employee.DepartmentId == departmentId.Value);
        if (branchId.HasValue)
            query = query.Where(a => a.Employee.BranchId == branchId.Value);
        if (employeeId.HasValue)
            query = query.Where(a => a.EmployeeId == employeeId.Value);

        var items = await query.OrderBy(a => a.Employee.FullName).ToListAsync();
        return items.Select(a => new HrDailyAttendanceDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee?.FullName,
            EmployeeNumber = a.Employee?.EmployeeNumber,
            AttendanceDate = a.AttendanceDate,
            ShiftName = a.WorkShift?.ShiftName,
            FirstCheckIn = a.FirstCheckIn,
            LastCheckOut = a.LastCheckOut,
            TotalWorkHours = a.TotalWorkHours,
            OvertimeHours = a.OvertimeHours,
            LateMinutes = a.LateMinutes,
            EarlyLeaveMinutes = a.EarlyLeaveMinutes,
            Status = a.Status,
            TotalDeductionAmount = a.TotalDeductionAmount,
            OvertimeAmount = a.OvertimeAmount,
            IsProcessed = a.IsProcessed,
            Notes = a.Notes
        }).ToList();
    }

    public async Task<List<HrDailyAttendanceDto>> GetMonthlyAttendanceAsync(int employeeId, int month, int year)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var items = await _attendanceRepo.Query()
            .Include(a => a.Employee)
            .Include(a => a.WorkShift)
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
            .OrderBy(a => a.AttendanceDate)
            .ToListAsync();

        return items.Select(a => new HrDailyAttendanceDto
        {
            Id = a.Id, EmployeeId = a.EmployeeId, EmployeeName = a.Employee?.FullName,
            AttendanceDate = a.AttendanceDate, ShiftName = a.WorkShift?.ShiftName,
            FirstCheckIn = a.FirstCheckIn, LastCheckOut = a.LastCheckOut,
            TotalWorkHours = a.TotalWorkHours, OvertimeHours = a.OvertimeHours,
            LateMinutes = a.LateMinutes, EarlyLeaveMinutes = a.EarlyLeaveMinutes,
            Status = a.Status, TotalDeductionAmount = a.TotalDeductionAmount,
            OvertimeAmount = a.OvertimeAmount, IsProcessed = a.IsProcessed
        }).ToList();
    }

    public async Task ProcessDailyAttendanceAsync(DateTime date)
    {
        var employees = await _employeeRepo.Query()
            .Include(e => e.WorkShift)
            .Where(e => e.Status == HrEmployeeStatus.Active)
            .ToListAsync();

        var holidays = await _holidayRepo.Query()
            .Where(h => h.StartDate <= date && h.EndDate >= date && h.IsActive)
            .ToListAsync();

        var isHoliday = holidays.Any();

        foreach (var employee in employees)
        {
            var existing = await _attendanceRepo.Query()
                .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.AttendanceDate == date.Date);

            if (existing != null) continue;

            var records = await _fingerprintRepo.Query()
                .Where(r => r.EmployeeId == employee.Id && r.RecordDate == date.Date)
                .OrderBy(r => r.RecordTime)
                .ToListAsync();

            var onLeave = await _leaveRepo.Query()
                .AnyAsync(l => l.EmployeeId == employee.Id &&
                    l.StartDate <= date && l.EndDate >= date &&
                    (l.Status == HrLeaveStatus.ApprovedByManager || l.Status == HrLeaveStatus.ApprovedByHR));

            var shift = employee.WorkShift;
            var attendance = new HrDailyAttendance
            {
                EmployeeId = employee.Id,
                AttendanceDate = date.Date,
                WorkShiftId = employee.WorkShiftId,
                ScheduledStart = shift?.StartTime,
                ScheduledEnd = shift?.EndTime
            };

            if (isHoliday)
            {
                attendance.Status = DailyAttendanceStatus.Holiday;
                attendance.IsHoliday = true;
            }
            else if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            {
                attendance.Status = DailyAttendanceStatus.Weekend;
                attendance.IsWeekend = true;
            }
            else if (onLeave)
            {
                attendance.Status = DailyAttendanceStatus.OnLeave;
                attendance.IsOnLeave = true;
            }
            else if (!records.Any())
            {
                attendance.Status = DailyAttendanceStatus.Absent;
                attendance.IsAbsent = true;
            }
            else
            {
                var firstCheckIn = records.FirstOrDefault(r => r.Type == FingerprintType.CheckIn);
                var lastCheckOut = records.LastOrDefault(r => r.Type == FingerprintType.CheckOut);

                attendance.FirstCheckIn = firstCheckIn?.RecordTime;
                attendance.LastCheckOut = lastCheckOut?.RecordTime;

                if (attendance.FirstCheckIn.HasValue && attendance.LastCheckOut.HasValue)
                {
                    attendance.TotalWorkHours = (decimal)(attendance.LastCheckOut.Value - attendance.FirstCheckIn.Value).TotalHours;
                }

                if (shift != null)
                {
                    attendance.RequiredWorkHours = shift.TotalWorkHours;

                    if (attendance.FirstCheckIn.HasValue && attendance.FirstCheckIn.Value > shift.StartTime.Add(TimeSpan.FromMinutes(shift.GracePeriodMinutes)))
                    {
                        attendance.IsLate = true;
                        attendance.LateMinutes = (int)(attendance.FirstCheckIn.Value - shift.StartTime).TotalMinutes;
                    }

                    if (attendance.LastCheckOut.HasValue && attendance.LastCheckOut.Value < shift.EndTime.Subtract(TimeSpan.FromMinutes(shift.EarlyLeaveGraceMinutes)))
                    {
                        attendance.IsEarlyLeave = true;
                        attendance.EarlyLeaveMinutes = (int)(shift.EndTime - attendance.LastCheckOut.Value).TotalMinutes;
                    }

                    if (attendance.TotalWorkHours > shift.TotalWorkHours)
                    {
                        attendance.IsOvertime = true;
                        attendance.OvertimeHours = attendance.TotalWorkHours - shift.TotalWorkHours;
                    }
                }

                attendance.Status = attendance.IsLate && attendance.IsEarlyLeave
                    ? DailyAttendanceStatus.LateAndEarlyLeave
                    : attendance.IsLate ? DailyAttendanceStatus.Late
                    : attendance.IsEarlyLeave ? DailyAttendanceStatus.EarlyLeave
                    : DailyAttendanceStatus.Present;
            }

            await _attendanceRepo.AddAsync(attendance);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
