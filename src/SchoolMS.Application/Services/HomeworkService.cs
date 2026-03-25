using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HomeworkService : IHomeworkService
{
    private readonly IRepository<Homework> _repository;
    private readonly IRepository<HomeworkAttachment> _attachmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HomeworkService(IRepository<Homework> repository, IRepository<HomeworkAttachment> attachmentRepository,
        IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _attachmentRepository = attachmentRepository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<HomeworkDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(h => h.Teacher).Include(h => h.ClassRoom).ThenInclude(c => c.Grade)
            .Include(h => h.ClassRoom).ThenInclude(c => c.Division)
            .Include(h => h.Subject).Include(h => h.AcademicYear)
            .Include(h => h.Attachments)
            .ToListAsync();
        return items.Select(h => new HomeworkDto
        {
            Id = h.Id, TeacherId = h.TeacherId, TeacherName = h.Teacher?.FullName,
            ClassRoomId = h.ClassRoomId, GradeName = h.ClassRoom?.Grade?.GradeName,
            DivisionName = h.ClassRoom?.Division?.DivisionName,
            SubjectId = h.SubjectId, SubjectName = h.Subject?.SubjectName,
            Title = h.Title, Description = h.Description,
            StartDate = h.StartDate, DueDate = h.DueDate, AcademicYearId = h.AcademicYearId,
            Attachments = h.Attachments.Select(a => new HomeworkAttachmentDto
            {
                Id = a.Id, HomeworkId = a.HomeworkId, FileName = a.FileName,
                FileUrl = a.FileUrl, FileType = a.FileType
            }).ToList()
        }).ToList();
    }

    public async Task<List<HomeworkDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? classRoomId = null,
        int? subjectId = null, int? teacherId = null, int? academicYearId = null)
    {
        var query = _repository.Query().Where(h => h.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(h => h.ClassRoom.BranchId == branchId.Value);
        if (classRoomId.HasValue) query = query.Where(h => h.ClassRoomId == classRoomId.Value);
        if (subjectId.HasValue) query = query.Where(h => h.SubjectId == subjectId.Value);
        if (teacherId.HasValue) query = query.Where(h => h.TeacherId == teacherId.Value);
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query
            .Include(h => h.Teacher).Include(h => h.ClassRoom).ThenInclude(c => c.Grade)
            .Include(h => h.ClassRoom).ThenInclude(c => c.Division)
            .Include(h => h.Subject).Include(h => h.AcademicYear)
            .Include(h => h.Attachments)
            .OrderByDescending(h => h.DueDate)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<HomeworkDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId,
        int? subjectId = null, int? academicYearId = null)
    {
        if (classRoomIds.Count == 0) return new List<HomeworkDto>();
        var query = _repository.Query()
            .Where(h => h.SchoolId == schoolId && classRoomIds.Contains(h.ClassRoomId));
        if (subjectId.HasValue) query = query.Where(h => h.SubjectId == subjectId.Value);
        if (academicYearId.HasValue) query = query.Where(h => h.AcademicYearId == academicYearId.Value);
        var items = await query
            .Include(h => h.Teacher).Include(h => h.ClassRoom).ThenInclude(c => c.Grade)
            .Include(h => h.ClassRoom).ThenInclude(c => c.Division)
            .Include(h => h.Subject).Include(h => h.AcademicYear)
            .Include(h => h.Attachments)
            .OrderByDescending(h => h.DueDate)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<HomeworkDto?> GetByIdAsync(int id)
    {
        var h = await _repository.Query()
            .Include(h => h.Teacher).Include(h => h.ClassRoom).ThenInclude(c => c.Grade)
            .Include(h => h.ClassRoom).ThenInclude(c => c.Division).Include(h => h.Subject)
            .Include(h => h.Attachments)
            .FirstOrDefaultAsync(h => h.Id == id);
        if (h == null) return null;
        return new HomeworkDto
        {
            Id = h.Id, TeacherId = h.TeacherId, TeacherName = h.Teacher?.FullName,
            ClassRoomId = h.ClassRoomId, SubjectId = h.SubjectId, SubjectName = h.Subject?.SubjectName,
            Title = h.Title, Description = h.Description,
            StartDate = h.StartDate, DueDate = h.DueDate, AcademicYearId = h.AcademicYearId,
            Attachments = h.Attachments.Select(a => new HomeworkAttachmentDto
            {
                Id = a.Id, HomeworkId = a.HomeworkId, FileName = a.FileName,
                FileUrl = a.FileUrl, FileType = a.FileType
            }).ToList()
        };
    }

    public async Task<HomeworkDto> CreateAsync(HomeworkDto dto)
    {
        var entity = new Homework
        {
            TeacherId = dto.TeacherId, ClassRoomId = dto.ClassRoomId, SubjectId = dto.SubjectId,
            Title = dto.Title, Description = dto.Description,
            StartDate = dto.StartDate, DueDate = dto.DueDate, AcademicYearId = dto.AcademicYearId,
            SchoolId = dto.SchoolId
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task<HomeworkDto> UpdateAsync(HomeworkDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.TeacherId = dto.TeacherId; entity.ClassRoomId = dto.ClassRoomId;
        entity.SubjectId = dto.SubjectId; entity.Title = dto.Title; entity.Description = dto.Description;
        entity.StartDate = dto.StartDate; entity.DueDate = dto.DueDate;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddAttachmentsAsync(int homeworkId, List<HomeworkAttachmentDto> attachments)
    {
        var homework = await _repository.GetByIdAsync(homeworkId);
        var schoolId = homework?.SchoolId ?? 0;
        foreach (var att in attachments)
        {
            var entity = new HomeworkAttachment
            {
                HomeworkId = homeworkId,
                FileName = att.FileName,
                FileUrl = att.FileUrl,
                FileType = att.FileType,
                SchoolId = schoolId
            };
            await _attachmentRepository.AddAsync(entity);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    private static HomeworkDto MapToDto(Homework h) => new()
    {
        Id = h.Id, TeacherId = h.TeacherId, TeacherName = h.Teacher?.FullName,
        ClassRoomId = h.ClassRoomId, GradeName = h.ClassRoom?.Grade?.GradeName,
        DivisionName = h.ClassRoom?.Division?.DivisionName,
        SubjectId = h.SubjectId, SubjectName = h.Subject?.SubjectName,
        Title = h.Title, Description = h.Description,
        StartDate = h.StartDate, DueDate = h.DueDate, AcademicYearId = h.AcademicYearId,
        Attachments = h.Attachments.Select(a => new HomeworkAttachmentDto
        {
            Id = a.Id, HomeworkId = a.HomeworkId, FileName = a.FileName,
            FileUrl = a.FileUrl, FileType = a.FileType
        }).ToList()
    };
}

