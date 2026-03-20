using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ExamTypeService : IExamTypeService
{
    private readonly IRepository<ExamType> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamTypeService(IRepository<ExamType> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<ExamTypeDto>> GetAllAsync()
        => _mapper.Map<List<ExamTypeDto>>(await _repository.Query().Include(e => e.School).ToListAsync());

    public async Task<List<ExamTypeDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query().Where(e => e.SchoolId == schoolId).Include(e => e.School).ToListAsync();
        return _mapper.Map<List<ExamTypeDto>>(items);
    }

    public async Task<ExamTypeDto?> GetByIdAsync(int id)
    { var e = await _repository.GetByIdAsync(id); return e == null ? null : _mapper.Map<ExamTypeDto>(e); }

    public async Task<ExamTypeDto> CreateAsync(ExamTypeDto dto)
    { var e = _mapper.Map<ExamType>(dto); e.Id = 0; await _repository.AddAsync(e); await _unitOfWork.SaveChangesAsync(); return _mapper.Map<ExamTypeDto>(e); }

    public async Task<ExamTypeDto> UpdateAsync(ExamTypeDto dto)
    {
        var e = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        e.TypeName = dto.TypeName;
        if (dto.SchoolId > 0) e.SchoolId = dto.SchoolId;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync(); return _mapper.Map<ExamTypeDto>(e);
    }

    public async Task DeleteAsync(int id)
    { var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException(); e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; _repository.Update(e); await _unitOfWork.SaveChangesAsync(); }
}

