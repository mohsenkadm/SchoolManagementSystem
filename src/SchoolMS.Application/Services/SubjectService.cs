using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly IRepository<Subject> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubjectService(IRepository<Subject> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<SubjectDto>> GetAllAsync()
        => _mapper.Map<List<SubjectDto>>(await _repository.Query().Include(s => s.School).ToListAsync());

    public async Task<List<SubjectDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(s => s.SchoolId == schoolId)
            .Include(s => s.School).ToListAsync();
        return _mapper.Map<List<SubjectDto>>(items);
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<SubjectDto>(entity);
    }

    public async Task<SubjectDto> CreateAsync(SubjectDto dto)
    {
        var entity = _mapper.Map<Subject>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SubjectDto>(entity);
    }

    public async Task<SubjectDto> UpdateAsync(SubjectDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Subject with ID {dto.Id} not found.");
        entity.SubjectName = dto.SubjectName;
        entity.Description = dto.Description;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SubjectDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Subject with ID {id} not found.");
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
