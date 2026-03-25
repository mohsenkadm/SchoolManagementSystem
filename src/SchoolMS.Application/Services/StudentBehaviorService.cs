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
    private readonly IRepository<AcademicYear> _academicYearRepo;
    private readonly IUnitOfWork _unitOfWork;

    public StudentBehaviorService(IRepository<StudentBehavior> repository, IRepository<Student> studentRepo,
        IRepository<AcademicYear> academicYearRepo, IUnitOfWork unitOfWork)
    { _repository = repository; _studentRepo = studentRepo; _academicYearRepo = academicYearRepo; _unitOfWork = unitOfWork; }

    private async Task<int?> ResolveCurrentAcademicYearIdAsync(int schoolId)
    {
        var year = await _academicYearRepo.Query()
            .FirstOrDefaultAsync(a => a.SchoolId == schoolId && a.IsCurrent);
        return year?.Id;
    }

    public async Task<List<StudentBehaviorDto>> GetAllAsync(int? academicYearId = null)
    {
        var query = _repository.Query().Include(b => b.Student).Include(b => b.AcademicYear).AsQueryable();
        if (academicYearId.HasValue) query = query.Where(b => b.AcademicYearId == academicYearId.Value);
        var items = await query.OrderByDescending(b => b.IncidentDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<StudentBehaviorDto>> GetByStudentIdAsync(int studentId, int? academicYearId = null)
    {
        var query = _repository.Query().Where(b => b.StudentId == studentId).Include(b => b.Student).Include(b => b.AcademicYear).AsQueryable();
        if (academicYearId.HasValue) query = query.Where(b => b.AcademicYearId == academicYearId.Value);
        var items = await query.OrderByDescending(b => b.IncidentDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<StudentBehaviorDto>> GetByParentChildrenAsync(int parentId, int schoolId, int? academicYearId = null)
    {
        var childrenIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.Id)
            .ToListAsync();

        if (childrenIds.Count == 0) return new List<StudentBehaviorDto>();

        var query = _repository.Query()
            .Where(b => childrenIds.Contains(b.StudentId) && b.SchoolId == schoolId)
            .Include(b => b.Student).Include(b => b.AcademicYear).AsQueryable();
        if (academicYearId.HasValue) query = query.Where(b => b.AcademicYearId == academicYearId.Value);
        var items = await query.OrderByDescending(b => b.IncidentDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<StudentBehaviorDto?> GetByIdAsync(int id)
    {
        var b = await _repository.Query().Include(x => x.Student).Include(x => x.AcademicYear).FirstOrDefaultAsync(x => x.Id == id);
        return b == null ? null : MapToDto(b);
    }

    public async Task<StudentBehaviorDto> CreateAsync(StudentBehaviorDto dto)
    {
        var academicYearId = await ResolveCurrentAcademicYearIdAsync(dto.SchoolId);
        var entity = new StudentBehavior
        {
            StudentId = dto.StudentId, Type = dto.Type, Title = dto.Title, Description = dto.Description,
            Points = dto.Points, ActionTaken = dto.ActionTaken, RecordedBy = dto.RecordedBy,
            IncidentDate = dto.IncidentDate, NotifyParent = dto.NotifyParent,
            SchoolId = dto.SchoolId, AcademicYearId = academicYearId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.AcademicYearId = academicYearId; return dto;
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
        SchoolId = b.SchoolId, AcademicYearId = b.AcademicYearId,
        AcademicYearName = b.AcademicYear?.YearName
    };
}

