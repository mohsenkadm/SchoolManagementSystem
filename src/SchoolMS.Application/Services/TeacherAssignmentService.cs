using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly IRepository<TeacherAssignment> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherAssignmentService(IRepository<TeacherAssignment> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<TeacherAssignmentDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(t => t.Teacher).Include(t => t.ClassRoom).ThenInclude(c => c.Grade)
            .Include(t => t.ClassRoom).ThenInclude(c => c.Division)
            .Include(t => t.Subject).Include(t => t.AcademicYear).Include(t => t.Branch)
            .Include(t => t.School)
            .ToListAsync();
        return _mapper.Map<List<TeacherAssignmentDto>>(items);
    }

    public async Task<List<TeacherAssignmentDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(t => t.SchoolId == schoolId)
            .Include(t => t.Teacher).Include(t => t.ClassRoom).ThenInclude(c => c.Grade)
            .Include(t => t.ClassRoom).ThenInclude(c => c.Division)
            .Include(t => t.Subject).Include(t => t.AcademicYear).Include(t => t.Branch)
            .Include(t => t.School)
            .ToListAsync();
        return _mapper.Map<List<TeacherAssignmentDto>>(items);
    }

    public async Task<List<TeacherAssignmentDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId)
    {
        if (classRoomIds.Count == 0) return new List<TeacherAssignmentDto>();
        var items = await _repository.Query()
            .Where(t => t.SchoolId == schoolId && classRoomIds.Contains(t.ClassRoomId))
            .Include(t => t.Teacher).Include(t => t.ClassRoom).ThenInclude(c => c.Grade)
            .Include(t => t.ClassRoom).ThenInclude(c => c.Division)
            .Include(t => t.Subject).Include(t => t.AcademicYear).Include(t => t.Branch)
            .Include(t => t.School)
            .ToListAsync();
        return _mapper.Map<List<TeacherAssignmentDto>>(items);
    }

    public async Task<TeacherAssignmentDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(t => t.Teacher).Include(t => t.ClassRoom).ThenInclude(c => c.Grade)
            .Include(t => t.ClassRoom).ThenInclude(c => c.Division)
            .Include(t => t.Subject).Include(t => t.AcademicYear).Include(t => t.Branch)
            .FirstOrDefaultAsync(t => t.Id == id);
        return entity == null ? null : _mapper.Map<TeacherAssignmentDto>(entity);
    }

    public async Task<TeacherAssignmentDto> CreateAsync(TeacherAssignmentDto dto)
    {
        var entity = _mapper.Map<TeacherAssignment>(dto);
        entity.Id = 0;
        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TeacherAssignmentDto>(entity);
    }

    public async Task<TeacherAssignmentDto> UpdateAsync(TeacherAssignmentDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.TeacherId = dto.TeacherId;
        entity.ClassRoomId = dto.ClassRoomId;
        entity.SubjectId = dto.SubjectId;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.BranchId = dto.BranchId;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TeacherAssignmentDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}

