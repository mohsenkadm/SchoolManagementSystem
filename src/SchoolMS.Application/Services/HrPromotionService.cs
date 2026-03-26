using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrPromotionService : IHrPromotionService
{
    private readonly IRepository<HrPromotion> _promotionRepo;
    private readonly IRepository<HrCareerHistory> _historyRepo;
    private readonly IRepository<HrEmployee> _employeeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrPromotionService(IRepository<HrPromotion> promotionRepo, IRepository<HrCareerHistory> historyRepo,
        IRepository<HrEmployee> employeeRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _promotionRepo = promotionRepo;
        _historyRepo = historyRepo;
        _employeeRepo = employeeRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrPromotionDto>> GetAllAsync(HrPromotionStatus? status = null, int? employeeId = null)
    {
        var query = _promotionRepo.Query().Include(p => p.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        if (employeeId.HasValue) query = query.Where(p => p.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(p => p.EffectiveDate).ToListAsync();
        return _mapper.Map<List<HrPromotionDto>>(items);
    }

    public async Task<List<HrPromotionDto>> GetBySchoolIdAsync(int schoolId, HrPromotionStatus? status = null, int? employeeId = null)
    {
        var query = _promotionRepo.Query().Where(p => p.SchoolId == schoolId).Include(p => p.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);
        if (employeeId.HasValue) query = query.Where(p => p.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(p => p.EffectiveDate).ToListAsync();
        return _mapper.Map<List<HrPromotionDto>>(items);
    }

    public async Task<HrPromotionDto?> GetByIdAsync(int id)
    {
        var entity = await _promotionRepo.Query().Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id);
        return entity == null ? null : _mapper.Map<HrPromotionDto>(entity);
    }

    public async Task<HrPromotionDto> CreateAsync(HrPromotionDto dto)
    {
        var entity = _mapper.Map<HrPromotion>(dto);
        entity.Id = 0;
        entity.Status = HrPromotionStatus.Pending;
        await _promotionRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrPromotionDto>(entity);
    }

    public async Task ApproveAsync(int id, string approvedBy)
    {
        var entity = await _promotionRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Promotion with ID {id} not found.");
        entity.Status = HrPromotionStatus.Effective;
        entity.ApprovedBy = approvedBy;
        entity.ApprovalDate = DateTime.UtcNow;
        _promotionRepo.Update(entity);

        // Update employee record
        var employee = await _employeeRepo.GetByIdAsync(entity.EmployeeId);
        if (employee != null)
        {
            if (entity.ToJobTitleId.HasValue) employee.JobTitleId = entity.ToJobTitleId.Value;
            if (entity.ToJobGradeId.HasValue) employee.JobGradeId = entity.ToJobGradeId;
            if (entity.ToJobGradeStepId.HasValue) employee.JobGradeStepId = entity.ToJobGradeStepId;
            if (entity.ToDepartmentId.HasValue) employee.DepartmentId = entity.ToDepartmentId.Value;
            if (entity.ToBranchId.HasValue) employee.BranchId = entity.ToBranchId.Value;
            _employeeRepo.Update(employee);
        }

        // Add career history
        var history = new HrCareerHistory
        {
            EmployeeId = entity.EmployeeId,
            EventType = "Promoted",
            Title = $"Promotion: {entity.Type}",
            Description = entity.Reason,
            EventDate = entity.EffectiveDate,
            OldValue = entity.FromSalary?.ToString("C"),
            NewValue = entity.ToSalary?.ToString("C"),
            DecisionNumber = entity.DecisionNumber,
            ProcessedBy = approvedBy
        };
        await _historyRepo.AddAsync(history);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectAsync(int id, string rejectedBy, string reason)
    {
        var entity = await _promotionRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Promotion with ID {id} not found.");
        entity.Status = HrPromotionStatus.Rejected;
        entity.ApprovedBy = rejectedBy;
        entity.Notes = reason;
        _promotionRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrCareerHistoryDto>> GetCareerHistoryAsync(int employeeId)
    {
        var items = await _historyRepo.Query()
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.EventDate)
            .ToListAsync();
        return _mapper.Map<List<HrCareerHistoryDto>>(items);
    }
}
