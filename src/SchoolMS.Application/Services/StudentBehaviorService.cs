using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class StudentBehaviorService : IStudentBehaviorService
{
    private readonly IRepository<StudentBehavior> _repository;
    private readonly IRepository<Student> _studentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public StudentBehaviorService(IRepository<StudentBehavior> repository, IRepository<Student> studentRepo, IUnitOfWork unitOfWork)
    { _repository = repository; _studentRepo = studentRepo; _unitOfWork = unitOfWork; }

    public async Task<List<StudentBehaviorDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(b => b.Student).OrderByDescending(b => b.IncidentDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<StudentBehaviorDto>> GetByStudentIdAsync(int studentId)
    {
        var items = await _repository.Query().Where(b => b.StudentId == studentId).Include(b => b.Student).OrderByDescending(b => b.IncidentDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<StudentBehaviorDto>> GetByParentChildrenAsync(int parentId, int schoolId)
    {
        var childrenIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.Id)
            .ToListAsync();

        if (childrenIds.Count == 0) return new List<StudentBehaviorDto>();

        var items = await _repository.Query()
            .Where(b => childrenIds.Contains(b.StudentId) && b.SchoolId == schoolId)
            .Include(b => b.Student)
            .OrderByDescending(b => b.IncidentDate)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<StudentBehaviorDto?> GetByIdAsync(int id)
    {
        var b = await _repository.Query().Include(x => x.Student).FirstOrDefaultAsync(x => x.Id == id);
        return b == null ? null : MapToDto(b);
    }

    public async Task<StudentBehaviorDto> CreateAsync(StudentBehaviorDto dto)
    {
        var entity = new StudentBehavior
        {
            StudentId = dto.StudentId, Type = dto.Type, Title = dto.Title, Description = dto.Description,
            Points = dto.Points, ActionTaken = dto.ActionTaken, RecordedBy = dto.RecordedBy,
            IncidentDate = dto.IncidentDate, NotifyParent = dto.NotifyParent,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task<StudentBehaviorDto> UpdateAsync(StudentBehaviorDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.Type = dto.Type; entity.Title = dto.Title; entity.Description = dto.Description;
        entity.Points = dto.Points; entity.ActionTaken = dto.ActionTaken; entity.IncidentDate = dto.IncidentDate;
        entity.NotifyParent = dto.NotifyParent; entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    private static StudentBehaviorDto MapToDto(StudentBehavior b) => new()
    {
        Id = b.Id, StudentId = b.StudentId, StudentName = b.Student?.FullName, Type = b.Type,
        Title = b.Title, Description = b.Description, Points = b.Points, ActionTaken = b.ActionTaken,
        RecordedBy = b.RecordedBy, IncidentDate = b.IncidentDate, NotifyParent = b.NotifyParent,
        SchoolId = b.SchoolId
    };
}

