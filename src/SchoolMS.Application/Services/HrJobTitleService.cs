using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrJobTitleService : IHrJobTitleService
{
    private readonly IRepository<HrJobTitle> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrJobTitleService(IRepository<HrJobTitle> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrJobTitleDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(j => j.Department).ToListAsync();
        return items.Select(j => new HrJobTitleDto
        {
            Id = j.Id,
            TitleName = j.TitleName,
            TitleNameAr = j.TitleNameAr,
            TitleCode = j.TitleCode,
            Description = j.Description,
            DepartmentId = j.DepartmentId,
            DepartmentName = j.Department?.DepartmentName,
            MinSalary = j.MinSalary,
            MaxSalary = j.MaxSalary,
            IsActive = j.IsActive
        }).ToList();
    }

    public async Task<HrJobTitleDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<HrJobTitleDto>(entity);
    }

    public async Task<HrJobTitleDto> CreateAsync(HrJobTitleDto dto)
    {
        var entity = _mapper.Map<HrJobTitle>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrJobTitleDto>(entity);
    }

    public async Task<HrJobTitleDto> UpdateAsync(HrJobTitleDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"JobTitle with ID {dto.Id} not found.");
        entity.TitleName = dto.TitleName;
        entity.TitleNameAr = dto.TitleNameAr;
        entity.TitleCode = dto.TitleCode;
        entity.Description = dto.Description;
        entity.DepartmentId = dto.DepartmentId;
        entity.MinSalary = dto.MinSalary;
        entity.MaxSalary = dto.MaxSalary;
        entity.IsActive = dto.IsActive;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrJobTitleDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"JobTitle with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
