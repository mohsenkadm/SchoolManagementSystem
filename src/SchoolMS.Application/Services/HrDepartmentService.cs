using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrDepartmentService : IHrDepartmentService
{
    private readonly IRepository<HrDepartment> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrDepartmentService(IRepository<HrDepartment> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrDepartmentDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(d => d.Branch)
            .Include(d => d.ParentDepartment)
            .Include(d => d.Employees)
            .ToListAsync();
        return MapDepartments(items);
    }

    public async Task<List<HrDepartmentDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(d => d.SchoolId == schoolId)
            .Include(d => d.Branch)
            .Include(d => d.ParentDepartment)
            .Include(d => d.Employees)
            .ToListAsync();
        return MapDepartments(items);
    }

    private static List<HrDepartmentDto> MapDepartments(List<HrDepartment> items)
    {
        return items.Select(d => new HrDepartmentDto
        {
            Id = d.Id,
            DepartmentName = d.DepartmentName,
            DepartmentNameAr = d.DepartmentNameAr,
            DepartmentCode = d.DepartmentCode,
            Description = d.Description,
            ParentDepartmentId = d.ParentDepartmentId,
            ParentDepartmentName = d.ParentDepartment?.DepartmentName,
            BranchId = d.BranchId,
            BranchName = d.Branch?.Name,
            IsActive = d.IsActive,
            Icon = d.Icon,
            Color = d.Color,
            EmployeeCount = d.Employees.Count
        }).ToList();
    }

    public async Task<HrDepartmentDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(d => d.Branch)
            .Include(d => d.ParentDepartment)
            .FirstOrDefaultAsync(d => d.Id == id);
        return entity == null ? null : _mapper.Map<HrDepartmentDto>(entity);
    }

    public async Task<HrDepartmentDto> CreateAsync(HrDepartmentDto dto)
    {
        var entity = _mapper.Map<HrDepartment>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDepartmentDto>(entity);
    }

    public async Task<HrDepartmentDto> UpdateAsync(HrDepartmentDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Department with ID {dto.Id} not found.");
        entity.DepartmentName = dto.DepartmentName;
        entity.DepartmentNameAr = dto.DepartmentNameAr;
        entity.DepartmentCode = dto.DepartmentCode;
        entity.Description = dto.Description;
        entity.ParentDepartmentId = dto.ParentDepartmentId;
        entity.ManagerEmployeeId = dto.ManagerEmployeeId;
        entity.BranchId = dto.BranchId;
        entity.IsActive = dto.IsActive;
        entity.Icon = dto.Icon;
        entity.Color = dto.Color;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrDepartmentDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Department with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
