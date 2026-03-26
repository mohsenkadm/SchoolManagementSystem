using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrOvertimeService : IHrOvertimeService
{
    private readonly IRepository<HrOvertimeRequest> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrOvertimeService(IRepository<HrOvertimeRequest> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrOvertimeRequestDto>> GetAllAsync(OvertimeStatus? status = null, int? employeeId = null)
    {
        var query = _repository.Query().Include(o => o.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(o => o.Status == status.Value);
        if (employeeId.HasValue) query = query.Where(o => o.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(o => o.OvertimeDate).ToListAsync();
        return MapOvertimeRequests(items);
    }

    public async Task<List<HrOvertimeRequestDto>> GetBySchoolIdAsync(int schoolId, OvertimeStatus? status = null, int? employeeId = null)
    {
        var query = _repository.Query().Where(o => o.SchoolId == schoolId).Include(o => o.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(o => o.Status == status.Value);
        if (employeeId.HasValue) query = query.Where(o => o.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(o => o.OvertimeDate).ToListAsync();
        return MapOvertimeRequests(items);
    }

    private static List<HrOvertimeRequestDto> MapOvertimeRequests(List<HrOvertimeRequest> items)
    {
        return items.Select(o => new HrOvertimeRequestDto
        {
            Id = o.Id, EmployeeId = o.EmployeeId, EmployeeName = o.Employee?.FullName,
            OvertimeDate = o.OvertimeDate, StartTime = o.StartTime, EndTime = o.EndTime,
            Hours = o.Hours, RateMultiplier = o.RateMultiplier, CalculatedAmount = o.CalculatedAmount,
            Reason = o.Reason, Status = o.Status, ApprovedBy = o.ApprovedBy,
            IsFromAttendance = o.IsFromAttendance, Notes = o.Notes
        }).ToList();
    }

    public async Task<HrOvertimeRequestDto> CreateAsync(HrOvertimeRequestDto dto)
    {
        var entity = _mapper.Map<HrOvertimeRequest>(dto);
        entity.Id = 0;
        entity.Status = OvertimeStatus.Pending;
        entity.Hours = (decimal)(dto.EndTime - dto.StartTime).TotalHours;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrOvertimeRequestDto>(entity);
    }

    public async Task ApproveAsync(int id, string approvedBy)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Overtime request with ID {id} not found.");
        entity.Status = OvertimeStatus.Approved;
        entity.ApprovedBy = approvedBy;
        entity.ApprovalDate = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, string rejectedBy, string reason)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Overtime request with ID {id} not found.");
        entity.Status = OvertimeStatus.Rejected;
        entity.ApprovedBy = rejectedBy;
        entity.RejectionReason = reason;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
