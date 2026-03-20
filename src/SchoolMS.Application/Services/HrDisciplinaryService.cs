using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrDisciplinaryService : IHrDisciplinaryService
{
    private readonly IRepository<HrDisciplinaryAction> _actionRepo;
    private readonly IRepository<HrViolationType> _typeRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrDisciplinaryService(IRepository<HrDisciplinaryAction> actionRepo, IRepository<HrViolationType> typeRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _actionRepo = actionRepo; _typeRepo = typeRepo; _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<HrDisciplinaryActionDto>> GetAllAsync(int? employeeId = null)
    {
        var query = _actionRepo.Query().Include(a => a.Employee).Include(a => a.ViolationType).AsQueryable();
        if (employeeId.HasValue) query = query.Where(a => a.EmployeeId == employeeId.Value);
        var items = await query.OrderByDescending(a => a.IssuedDate).ToListAsync();
        return items.Select(a => new HrDisciplinaryActionDto
        {
            Id = a.Id, EmployeeId = a.EmployeeId, EmployeeName = a.Employee?.FullName,
            ViolationTypeId = a.ViolationTypeId, ViolationName = a.ViolationType?.ViolationName,
            IncidentDate = a.IncidentDate, IncidentDescription = a.IncidentDescription,
            ActionType = a.ActionType, WarningLevel = a.WarningLevel,
            SuspensionDays = a.SuspensionDays, PenaltyAmount = a.PenaltyAmount,
            Status = a.Status, IssuedBy = a.IssuedBy, IssuedDate = a.IssuedDate, Notes = a.Notes
        }).ToList();
    }

    public async Task<HrDisciplinaryActionDto?> GetByIdAsync(int id)
    {
        var entity = await _actionRepo.Query().Include(a => a.Employee).Include(a => a.ViolationType)
            .FirstOrDefaultAsync(a => a.Id == id);
        return entity == null ? null : _mapper.Map<HrDisciplinaryActionDto>(entity);
    }

    public async Task<HrDisciplinaryActionDto> CreateAsync(HrDisciplinaryActionDto dto)
    {
        var entity = _mapper.Map<HrDisciplinaryAction>(dto); entity.Id = 0;
        entity.Status = DisciplinaryStatus.Issued; entity.IssuedDate = DateTime.UtcNow;
        await _actionRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDisciplinaryActionDto>(entity);
    }

    public async Task<HrDisciplinaryActionDto> UpdateAsync(HrDisciplinaryActionDto dto)
    {
        var entity = await _actionRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Action {dto.Id} not found.");
        entity.IncidentDescription = dto.IncidentDescription; entity.ActionType = dto.ActionType;
        entity.WarningLevel = dto.WarningLevel; entity.SuspensionDays = dto.SuspensionDays;
        entity.PenaltyAmount = dto.PenaltyAmount; entity.Status = dto.Status; entity.Notes = dto.Notes;
        _actionRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDisciplinaryActionDto>(entity);
    }

    public async Task<List<HrViolationTypeDto>> GetViolationTypesAsync()
        => _mapper.Map<List<HrViolationTypeDto>>(await _typeRepo.GetAllAsync());

    public async Task<HrViolationTypeDto> CreateViolationTypeAsync(HrViolationTypeDto dto)
    {
        var entity = _mapper.Map<HrViolationType>(dto); entity.Id = 0;
        await _typeRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrViolationTypeDto>(entity);
    }

    public async Task<HrViolationTypeDto> UpdateViolationTypeAsync(HrViolationTypeDto dto)
    {
        var entity = await _typeRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"ViolationType {dto.Id} not found.");
        entity.ViolationName = dto.ViolationName; entity.ViolationNameAr = dto.ViolationNameAr;
        entity.Description = dto.Description; entity.Severity = dto.Severity;
        entity.DefaultAction = dto.DefaultAction; entity.DefaultPenaltyAmount = dto.DefaultPenaltyAmount;
        entity.DefaultSuspensionDays = dto.DefaultSuspensionDays; entity.IsActive = dto.IsActive;
        _typeRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrViolationTypeDto>(entity);
    }

    public async Task DeleteViolationTypeAsync(int id)
    {
        var entity = await _typeRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"ViolationType {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _typeRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }
}
