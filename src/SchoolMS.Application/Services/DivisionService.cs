using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class DivisionService : IDivisionService
{
    private readonly IRepository<Division> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DivisionService(IRepository<Division> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<DivisionDto>> GetAllAsync()
        => _mapper.Map<List<DivisionDto>>(await _repository.Query().Include(d => d.School).ToListAsync());

    public async Task<List<DivisionDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(d => d.SchoolId == schoolId)
            .Include(d => d.School).ToListAsync();
        return _mapper.Map<List<DivisionDto>>(items);
    }

    public async Task<DivisionDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<DivisionDto>(entity);
    }

    public async Task<DivisionDto> CreateAsync(DivisionDto dto)
    {
        var entity = _mapper.Map<Division>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<DivisionDto>(entity);
    }

    public async Task<DivisionDto> UpdateAsync(DivisionDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Division with ID {dto.Id} not found.");
        entity.DivisionName = dto.DivisionName;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<DivisionDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Division with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
