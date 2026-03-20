using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class GradeService : IGradeService
{
    private readonly IRepository<Grade> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GradeService(IRepository<Grade> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<GradeDto>> GetAllAsync()
    {
        var grades = await _repository.Query()
            .Include(g => g.Division).Include(g => g.Branch).Include(g => g.School)
            .ToListAsync();
        return _mapper.Map<List<GradeDto>>(grades);
    }

    public async Task<List<GradeDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = _repository.Query().Where(g => g.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(g => g.BranchId == branchId.Value);
        var grades = await query
            .Include(g => g.Division).Include(g => g.Branch).Include(g => g.School)
            .ToListAsync();
        return _mapper.Map<List<GradeDto>>(grades);
    }

    public async Task<GradeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(g => g.Division).Include(g => g.Branch)
            .FirstOrDefaultAsync(g => g.Id == id);
        return entity == null ? null : _mapper.Map<GradeDto>(entity);
    }

    public async Task<GradeDto> CreateAsync(GradeDto dto)
    {
        var entity = _mapper.Map<Grade>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GradeDto>(entity);
    }

    public async Task<GradeDto> UpdateAsync(GradeDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Grade with ID {dto.Id} not found.");
        entity.GradeName = dto.GradeName;
        entity.DivisionId = dto.DivisionId;
        entity.BranchId = dto.BranchId;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GradeDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Grade with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
