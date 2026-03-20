using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrDashboardService : IHrDashboardService
{
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IRepository<HrDailyAttendance> _attendanceRepo;
    private readonly IRepository<HrLeaveRequest> _leaveRepo;
    private readonly IRepository<HrSalaryAdvance> _advanceRepo;
    private readonly IRepository<HrOvertimeRequest> _overtimeRepo;
    private readonly IRepository<HrEmployeeContract> _contractRepo;
    private readonly IRepository<HrEmployeeDocument> _documentRepo;
    private readonly IRepository<HrPayrollItem> _payrollItemRepo;

    public HrDashboardService(IRepository<HrEmployee> employeeRepo, IRepository<HrDailyAttendance> attendanceRepo,
        IRepository<HrLeaveRequest> leaveRepo, IRepository<HrSalaryAdvance> advanceRepo,
        IRepository<HrOvertimeRequest> overtimeRepo, IRepository<HrEmployeeContract> contractRepo,
        IRepository<HrEmployeeDocument> documentRepo, IRepository<HrPayrollItem> payrollItemRepo)
    {
        _employeeRepo = employeeRepo; _attendanceRepo = attendanceRepo; _leaveRepo = leaveRepo;
        _advanceRepo = advanceRepo; _overtimeRepo = overtimeRepo; _contractRepo = contractRepo;
        _documentRepo = documentRepo; _payrollItemRepo = payrollItemRepo;
    }

    public async Task<HrDashboardDto> GetDashboardAsync(int? branchId = null)
    {
        var today = DateTime.UtcNow.Date;
        var employeeQuery = _employeeRepo.Query().Where(e => e.Status == HrEmployeeStatus.Active);
        if (branchId.HasValue) employeeQuery = employeeQuery.Where(e => e.BranchId == branchId.Value);

        var employees = await employeeQuery.Include(e => e.Department).ToListAsync();
        var todayAttendance = await _attendanceRepo.Query().Where(a => a.AttendanceDate == today).ToListAsync();

        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var monthAttendance = await _attendanceRepo.Query()
            .Where(a => a.AttendanceDate >= monthStart && a.AttendanceDate <= monthEnd).ToListAsync();

        var totalWorkDays = monthAttendance.Select(a => a.AttendanceDate).Distinct().Count();
        var presentDays = monthAttendance.Count(a => a.Status == DailyAttendanceStatus.Present ||
            a.Status == DailyAttendanceStatus.Late || a.Status == DailyAttendanceStatus.EarlyLeave);
        var totalPossible = totalWorkDays * employees.Count;

        var thisMonthPayroll = await _payrollItemRepo.Query()
            .Where(p => p.MonthlyPayroll.Month == now.Month && p.MonthlyPayroll.Year == now.Year)
            .SumAsync(p => (decimal?)p.NetSalary) ?? 0;

        var expiringContracts = await _contractRepo.Query()
            .CountAsync(c => c.EndDate.HasValue && c.EndDate.Value >= today && c.EndDate.Value <= today.AddDays(30) && c.Status == ContractStatus.Active);

        var expiringDocs = await _documentRepo.Query()
            .CountAsync(d => d.ExpiryDate.HasValue && d.ExpiryDate.Value >= today && d.ExpiryDate.Value <= today.AddDays(30));

        return new HrDashboardDto
        {
            TotalActiveEmployees = employees.Count,
            OnLeaveToday = todayAttendance.Count(a => a.IsOnLeave),
            AbsentToday = todayAttendance.Count(a => a.IsAbsent),
            LateToday = todayAttendance.Count(a => a.IsLate),
            AverageAttendanceRate = totalPossible > 0 ? Math.Round((decimal)presentDays / totalPossible * 100, 1) : 0,
            TotalPayrollThisMonth = thisMonthPayroll,
            PendingLeaves = await _leaveRepo.CountAsync(l => l.Status == HrLeaveStatus.Pending),
            PendingAdvances = await _advanceRepo.CountAsync(a => a.Status == AdvanceStatus.Pending),
            PendingOvertime = await _overtimeRepo.CountAsync(o => o.Status == OvertimeStatus.Pending),
            PendingRequests = await _leaveRepo.CountAsync(l => l.Status == HrLeaveStatus.Pending)
                + await _advanceRepo.CountAsync(a => a.Status == AdvanceStatus.Pending)
                + await _overtimeRepo.CountAsync(o => o.Status == OvertimeStatus.Pending),
            ExpiringContracts = expiringContracts,
            ExpiringDocuments = expiringDocs,
            EmployeesByDepartment = employees.GroupBy(e => e.Department?.DepartmentName ?? "Unknown")
                .ToDictionary(g => g.Key, g => g.Count()),
            EmployeesByType = employees.GroupBy(e => e.EmployeeType.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            EmployeesByStatus = employees.GroupBy(e => e.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}
