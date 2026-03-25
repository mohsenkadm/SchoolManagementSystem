using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrJobGradeService : IHrJobGradeService
{
    private readonly IRepository<HrJobGrade> _gradeRepo;
    private readonly IRepository<HrJobGradeStep> _stepRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrJobGradeService(IRepository<HrJobGrade> gradeRepo, IRepository<HrJobGradeStep> stepRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _gradeRepo = gradeRepo;
        _stepRepo = stepRepo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<HrJobGradeDto>> GetAllAsync()
        => _mapper.Map<List<HrJobGradeDto>>(await _gradeRepo.GetAllAsync());

    public async Task<List<HrJobGradeDto>> GetBySchoolIdAsync(int schoolId)
        => _mapper.Map<List<HrJobGradeDto>>(await _gradeRepo.Query().Where(g => g.SchoolId == schoolId).ToListAsync());

    public async Task<HrJobGradeDto?> GetByIdAsync(int id)
    {
        var entity = await _gradeRepo.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<HrJobGradeDto>(entity);
    }

    public async Task<HrJobGradeDto> CreateAsync(HrJobGradeDto dto)
    {
        var entity = _mapper.Map<HrJobGrade>(dto);
        entity.Id = 0;
        await _gradeRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrJobGradeDto>(entity);
    }

    public async Task<HrJobGradeDto> UpdateAsync(HrJobGradeDto dto)
    {
        var entity = await _gradeRepo.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"JobGrade with ID {dto.Id} not found.");
        entity.GradeName = dto.GradeName;
        entity.GradeNameAr = dto.GradeNameAr;
        entity.GradeLevel = dto.GradeLevel;
        entity.Description = dto.Description;
        entity.MinSalary = dto.MinSalary;
        entity.MaxSalary = dto.MaxSalary;
        entity.DefaultAllowancePercentage = dto.DefaultAllowancePercentage;
        entity.MinYearsExperience = dto.MinYearsExperience;
        entity.IsActive = dto.IsActive;
        _gradeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrJobGradeDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _gradeRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"JobGrade with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _gradeRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrJobGradeStepDto>> GetStepsAsync(int gradeId)
    {
        var steps = await _stepRepo.Query().Where(s => s.JobGradeId == gradeId).OrderBy(s => s.StepNumber).ToListAsync();
        return _mapper.Map<List<HrJobGradeStepDto>>(steps);
    }

    public async Task<HrJobGradeStepDto> CreateStepAsync(HrJobGradeStepDto dto)
    {
        var entity = _mapper.Map<HrJobGradeStep>(dto);
        entity.Id = 0;
        await _stepRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrJobGradeStepDto>(entity);
    }

    public async Task DeleteStepAsync(int stepId)
    {
        var entity = await _stepRepo.GetByIdAsync(stepId)
            ?? throw new KeyNotFoundException($"Step with ID {stepId} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _stepRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
