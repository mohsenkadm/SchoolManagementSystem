using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class BranchService : IBranchService
{
    private readonly IRepository<Branch> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BranchService(IRepository<Branch> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<BranchDto>> GetAllAsync()
    {
        var branches = await _repository.Query()
            .Include(b => b.School)
            .ToListAsync();
        return _mapper.Map<List<BranchDto>>(branches);
    }

    public async Task<List<BranchDto>> GetBySchoolIdAsync(int schoolId)
    {
        var branches = await _repository.Query()
            .Where(b => b.SchoolId == schoolId)
            .Include(b => b.School)
            .ToListAsync();
        return _mapper.Map<List<BranchDto>>(branches);
    }

    public async Task<BranchDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<BranchDto>(entity);
    }

    public async Task<BranchDto> CreateAsync(BranchDto dto)
    {
        var entity = _mapper.Map<Branch>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(entity);
    }

    public async Task<BranchDto> UpdateAsync(BranchDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Branch with ID {dto.Id} not found.");
        entity.Name = dto.Name;
        entity.Address = dto.Address;
        entity.IsActive = dto.IsActive;
        if (dto.SchoolId > 0)
            entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Branch with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
