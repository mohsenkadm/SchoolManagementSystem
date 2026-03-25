using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrLeaveService : IHrLeaveService
{
    private readonly IRepository<HrLeaveRequest> _requestRepo;
    private readonly IRepository<HrLeaveType> _typeRepo;
    private readonly IRepository<HrLeaveBalance> _balanceRepo;
    private readonly IRepository<HrHoliday> _holidayRepo;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrLeaveService(IRepository<HrLeaveRequest> requestRepo, IRepository<HrLeaveType> typeRepo,
        IRepository<HrLeaveBalance> balanceRepo, IRepository<HrHoliday> holidayRepo,
        IRepository<HrEmployee> employeeRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _requestRepo = requestRepo;
        _typeRepo = typeRepo;
        _balanceRepo = balanceRepo;
        _holidayRepo = holidayRepo;
        _employeeRepo = employeeRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrLeaveRequestDto>> GetAllRequestsAsync(HrLeaveStatus? status = null)
    {
        var query = _requestRepo.Query().Include(l => l.Employee).Include(l => l.LeaveType).AsQueryable();
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);
        var items = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
        return MapLeaveRequests(items);
    }

    public async Task<List<HrLeaveRequestDto>> GetRequestsBySchoolIdAsync(int schoolId, HrLeaveStatus? status = null)
    {
        var query = _requestRepo.Query().Where(l => l.SchoolId == schoolId).Include(l => l.Employee).Include(l => l.LeaveType).AsQueryable();
        if (status.HasValue) query = query.Where(l => l.Status == status.Value);
        var items = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
        return MapLeaveRequests(items);
    }

    public async Task<List<HrHolidayDto>> GetHolidaysBySchoolIdAsync(int schoolId, int? year = null)
    {
        var query = _holidayRepo.Query().Where(h => h.SchoolId == schoolId);
        if (year.HasValue) query = query.Where(h => h.StartDate.Year == year.Value);
        return _mapper.Map<List<HrHolidayDto>>(await query.OrderBy(h => h.StartDate).ToListAsync());
    }

    private static List<HrLeaveRequestDto> MapLeaveRequests(List<HrLeaveRequest> items)
    {
        return items.Select(l => new HrLeaveRequestDto
        {
            Id = l.Id, EmployeeId = l.EmployeeId, EmployeeName = l.Employee?.FullName,
            LeaveTypeId = l.LeaveTypeId, LeaveTypeName = l.LeaveType?.LeaveTypeName,
            StartDate = l.StartDate, EndDate = l.EndDate, TotalDays = l.TotalDays,
            IsHalfDay = l.IsHalfDay, Reason = l.Reason, Status = l.Status, Notes = l.Notes
        }).ToList();
    }

    public async Task<HrLeaveRequestDto> CreateRequestAsync(HrLeaveRequestDto dto)
    {
        var entity = _mapper.Map<HrLeaveRequest>(dto);
        entity.Id = 0;
        entity.Status = HrLeaveStatus.Pending;
        entity.TotalDays = dto.IsHalfDay ? 0.5m : (decimal)(dto.EndDate - dto.StartDate).TotalDays + 1;
        await _requestRepo.AddAsync(entity);

        var balance = await _balanceRepo.Query()
            .FirstOrDefaultAsync(b => b.EmployeeId == dto.EmployeeId && b.LeaveTypeId == dto.LeaveTypeId && b.Year == dto.StartDate.Year);
        if (balance != null)
        {
            balance.Pending += entity.TotalDays;
            balance.Remaining = balance.TotalAvailable - balance.Used - balance.Pending;
            _balanceRepo.Update(balance);
        }

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrLeaveRequestDto>(entity);
    }

    public async Task ApproveByManagerAsync(int id, string approvedBy)
    {
        var entity = await _requestRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Leave request with ID {id} not found.");
        entity.Status = HrLeaveStatus.ApprovedByManager;
        entity.ApprovedByManager = approvedBy;
        entity.ManagerApprovalDate = DateTime.UtcNow;
        _requestRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveByHrAsync(int id, string approvedBy)
    {
        var entity = await _requestRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Leave request with ID {id} not found.");
        entity.Status = HrLeaveStatus.ApprovedByHR;
        entity.ApprovedByHR = approvedBy;
        entity.HrApprovalDate = DateTime.UtcNow;
        _requestRepo.Update(entity);

        var balance = await _balanceRepo.Query()
            .FirstOrDefaultAsync(b => b.EmployeeId == entity.EmployeeId && b.LeaveTypeId == entity.LeaveTypeId && b.Year == entity.StartDate.Year);
        if (balance != null)
        {
            balance.Pending -= entity.TotalDays;
            balance.Used += entity.TotalDays;
            balance.Remaining = balance.TotalAvailable - balance.Used - balance.Pending;
            _balanceRepo.Update(balance);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, string rejectedBy, string reason)
    {
        var entity = await _requestRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Leave request with ID {id} not found.");
        entity.Status = HrLeaveStatus.Rejected;
        entity.RejectionReason = reason;
        _requestRepo.Update(entity);

        var balance = await _balanceRepo.Query()
            .FirstOrDefaultAsync(b => b.EmployeeId == entity.EmployeeId && b.LeaveTypeId == entity.LeaveTypeId && b.Year == entity.StartDate.Year);
        if (balance != null)
        {
            balance.Pending -= entity.TotalDays;
            balance.Remaining = balance.TotalAvailable - balance.Used - balance.Pending;
            _balanceRepo.Update(balance);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelAsync(int id)
    {
        var entity = await _requestRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Leave request with ID {id} not found.");
        entity.Status = HrLeaveStatus.Cancelled;
        _requestRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrLeaveTypeDto>> GetLeaveTypesAsync()
        => _mapper.Map<List<HrLeaveTypeDto>>(await _typeRepo.GetAllAsync());

    public async Task<HrLeaveTypeDto> CreateLeaveTypeAsync(HrLeaveTypeDto dto)
    {
        var entity = _mapper.Map<HrLeaveType>(dto);
        entity.Id = 0;
        await _typeRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrLeaveTypeDto>(entity);
    }

    public async Task<HrLeaveTypeDto> UpdateLeaveTypeAsync(HrLeaveTypeDto dto)
    {
        var entity = await _typeRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"LeaveType with ID {dto.Id} not found.");
        entity.LeaveTypeName = dto.LeaveTypeName;
        entity.LeaveTypeNameAr = dto.LeaveTypeNameAr;
        entity.LeaveCode = dto.LeaveCode;
        entity.DefaultDaysPerYear = dto.DefaultDaysPerYear;
        entity.IsPaid = dto.IsPaid;
        entity.RequiresApproval = dto.RequiresApproval;
        entity.RequiresDocument = dto.RequiresDocument;
        entity.DeductsFromSalary = dto.DeductsFromSalary;
        entity.DeductionPerDay = dto.DeductionPerDay;
        entity.AllowHalfDay = dto.AllowHalfDay;
        entity.AllowNegativeBalance = dto.AllowNegativeBalance;
        entity.MaxConsecutiveDays = dto.MaxConsecutiveDays;
        entity.CarryForward = dto.CarryForward;
        entity.MaxCarryForwardDays = dto.MaxCarryForwardDays;
        entity.Color = dto.Color;
        entity.ApplicableFor = dto.ApplicableFor;
        entity.IsActive = dto.IsActive;
        _typeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrLeaveTypeDto>(entity);
    }

    public async Task DeleteLeaveTypeAsync(int id)
    {
        var entity = await _typeRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"LeaveType with ID {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _typeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrLeaveBalanceDto>> GetBalancesAsync(int employeeId)
    {
        var items = await _balanceRepo.Query()
            .Include(b => b.Employee).Include(b => b.LeaveType)
            .Where(b => b.EmployeeId == employeeId).ToListAsync();
        return items.Select(b => new HrLeaveBalanceDto
        {
            Id = b.Id, EmployeeId = b.EmployeeId, EmployeeName = b.Employee?.FullName,
            LeaveTypeId = b.LeaveTypeId, LeaveTypeName = b.LeaveType?.LeaveTypeName,
            Year = b.Year, TotalEntitlement = b.TotalEntitlement, CarriedForward = b.CarriedForward,
            TotalAvailable = b.TotalAvailable, Used = b.Used, Pending = b.Pending, Remaining = b.Remaining
        }).ToList();
    }

    public async Task<List<HrLeaveBalanceDto>> GetAllBalancesAsync(int year)
    {
        var items = await _balanceRepo.Query()
            .Include(b => b.Employee).Include(b => b.LeaveType)
            .Where(b => b.Year == year).ToListAsync();
        return items.Select(b => new HrLeaveBalanceDto
        {
            Id = b.Id, EmployeeId = b.EmployeeId, EmployeeName = b.Employee?.FullName,
            LeaveTypeId = b.LeaveTypeId, LeaveTypeName = b.LeaveType?.LeaveTypeName,
            Year = b.Year, TotalEntitlement = b.TotalEntitlement, CarriedForward = b.CarriedForward,
            TotalAvailable = b.TotalAvailable, Used = b.Used, Pending = b.Pending, Remaining = b.Remaining
        }).ToList();
    }

    public async Task InitializeBalancesAsync(int employeeId, int year)
    {
        var leaveTypes = await _typeRepo.Query().Where(t => t.IsActive).ToListAsync();
        foreach (var lt in leaveTypes)
        {
            var exists = await _balanceRepo.AnyAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == lt.Id && b.Year == year);
            if (!exists)
            {
                await _balanceRepo.AddAsync(new HrLeaveBalance
                {
                    EmployeeId = employeeId, LeaveTypeId = lt.Id, Year = year,
                    TotalEntitlement = lt.DefaultDaysPerYear, TotalAvailable = lt.DefaultDaysPerYear,
                    Remaining = lt.DefaultDaysPerYear
                });
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrHolidayDto>> GetHolidaysAsync(int? year = null)
    {
        var query = _holidayRepo.Query().Include(h => h.Branch).AsQueryable();
        if (year.HasValue) query = query.Where(h => h.StartDate.Year == year.Value || h.EndDate.Year == year.Value);
        var items = await query.OrderBy(h => h.StartDate).ToListAsync();
        return _mapper.Map<List<HrHolidayDto>>(items);
    }

    public async Task<HrHolidayDto> CreateHolidayAsync(HrHolidayDto dto)
    {
        var entity = _mapper.Map<HrHoliday>(dto);
        entity.Id = 0;
        entity.TotalDays = (int)(dto.EndDate - dto.StartDate).TotalDays + 1;
        await _holidayRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrHolidayDto>(entity);
    }

    public async Task<HrHolidayDto> UpdateHolidayAsync(HrHolidayDto dto)
    {
        var entity = await _holidayRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Holiday with ID {dto.Id} not found.");
        entity.HolidayName = dto.HolidayName; entity.HolidayNameAr = dto.HolidayNameAr;
        entity.StartDate = dto.StartDate; entity.EndDate = dto.EndDate;
        entity.TotalDays = (int)(dto.EndDate - dto.StartDate).TotalDays + 1;
        entity.Type = dto.Type; entity.IsRecurring = dto.IsRecurring;
        entity.BranchId = dto.BranchId; entity.IsActive = dto.IsActive; entity.Notes = dto.Notes;
        _holidayRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrHolidayDto>(entity);
    }

    public async Task DeleteHolidayAsync(int id)
    {
        var entity = await _holidayRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Holiday with ID {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _holidayRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
