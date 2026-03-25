using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HealthRecordService : IHealthRecordService
{
    private readonly IRepository<HealthRecord> _repository;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<AcademicYear> _academicYearRepo;
    private readonly IUnitOfWork _unitOfWork;

    public HealthRecordService(IRepository<HealthRecord> repository, IRepository<Student> studentRepo,
        IRepository<AcademicYear> academicYearRepo, IUnitOfWork unitOfWork)
    { _repository = repository; _studentRepo = studentRepo; _academicYearRepo = academicYearRepo; _unitOfWork = unitOfWork; }

    private async Task<int?> ResolveCurrentAcademicYearIdAsync(int schoolId)
    {
        var year = await _academicYearRepo.Query()
            .FirstOrDefaultAsync(a => a.SchoolId == schoolId && a.IsCurrent);
        return year?.Id;
    }

    public async Task<List<HealthRecordDto>> GetAllAsync(int? academicYearId = null)
    {
        var query = _repository.Query().Include(h => h.Student).Include(h => h.AcademicYear).AsQueryable();
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query.OrderByDescending(h => h.RecordDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<HealthRecordDto>> GetBySchoolIdAsync(int schoolId, int? studentId = null, int? academicYearId = null)
    {
        var query = _repository.Query().Where(h => h.SchoolId == schoolId);
        if (studentId.HasValue) query = query.Where(h => h.StudentId == studentId.Value);
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query.Include(h => h.Student).Include(h => h.AcademicYear).OrderByDescending(h => h.RecordDate).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<HealthRecordDto>> GetByStudentIdAsync(int studentId, int? academicYearId = null)
    {
        var query = _repository.Query().Where(h => h.StudentId == studentId);
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query.Include(h => h.Student).Include(h => h.AcademicYear).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<HealthRecordDto>> GetByParentChildrenAsync(int parentId, int schoolId, int? academicYearId = null)
    {
        var childrenIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.Id)
            .ToListAsync();

        if (childrenIds.Count == 0) return new List<HealthRecordDto>();

        var query = _repository.Query()
            .Where(h => childrenIds.Contains(h.StudentId) && h.SchoolId == schoolId);
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query
            .Include(h => h.Student).Include(h => h.AcademicYear)
            .OrderByDescending(h => h.RecordDate)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<HealthRecordDto?> GetByIdAsync(int id)
    {
        var h = await _repository.Query().Include(x => x.Student).Include(x => x.AcademicYear).FirstOrDefaultAsync(x => x.Id == id);
        return h == null ? null : MapToDto(h);
    }

    public async Task<HealthRecordDto> CreateAsync(HealthRecordDto dto)
    {
        var academicYearId = await ResolveCurrentAcademicYearIdAsync(dto.SchoolId);
        var entity = new HealthRecord
        {
            StudentId = dto.StudentId, RecordDate = dto.RecordDate, RecordType = dto.RecordType,
            Title = dto.Title, Description = dto.Description, DoctorName = dto.DoctorName,
            Prescription = dto.Prescription, FollowUpDate = dto.FollowUpDate, NotifyParent = dto.NotifyParent,
            SchoolId = dto.SchoolId, AcademicYearId = academicYearId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; dto.AcademicYearId = academicYearId; return dto;
    }

    public async Task<HealthRecordDto> UpdateAsync(HealthRecordDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.RecordDate = dto.RecordDate; entity.RecordType = dto.RecordType; entity.Title = dto.Title;
        entity.Description = dto.Description; entity.DoctorName = dto.DoctorName;
        entity.Prescription = dto.Prescription; entity.FollowUpDate = dto.FollowUpDate;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync(); return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }

    private static HealthRecordDto MapToDto(HealthRecord h) => new()
    {
        Id = h.Id, StudentId = h.StudentId, StudentName = h.Student?.FullName, RecordDate = h.RecordDate,
        RecordType = h.RecordType, Title = h.Title, Description = h.Description, DoctorName = h.DoctorName,
        Prescription = h.Prescription, FollowUpDate = h.FollowUpDate, NotifyParent = h.NotifyParent,
        SchoolId = h.SchoolId, AcademicYearId = h.AcademicYearId,
        AcademicYearName = h.AcademicYear?.YearName
    };
}

