using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AcademicYearService : IAcademicYearService
{
    private readonly IRepository<AcademicYear> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AcademicYearService(IRepository<AcademicYear> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<AcademicYearDto>> GetAllAsync(int schoolId)
        => _mapper.Map<List<AcademicYearDto>>(await _repository.Query().Where(a => a.SchoolId == schoolId).ToListAsync());

    public async Task<AcademicYearDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<AcademicYearDto>(entity);
    }

    public async Task<AcademicYearDto?> GetCurrentAsync(int schoolId)
    {
        var entity = await _repository.Query().FirstOrDefaultAsync(a => a.SchoolId == schoolId && a.IsCurrent);
        return entity == null ? null : _mapper.Map<AcademicYearDto>(entity);
    }

    public async Task<AcademicYearDto> CreateAsync(AcademicYearDto dto)
    {
        var entity = _mapper.Map<AcademicYear>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AcademicYearDto>(entity);
    }

    public async Task<AcademicYearDto> UpdateAsync(AcademicYearDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"AcademicYear with ID {dto.Id} not found.");
        entity.YearName = dto.YearName;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsCurrent = dto.IsCurrent;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AcademicYearDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"AcademicYear with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
