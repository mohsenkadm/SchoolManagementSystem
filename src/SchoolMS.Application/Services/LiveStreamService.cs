using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class LiveStreamService : ILiveStreamService
{
    private readonly IRepository<LiveStream> _repository;
    private readonly IRepository<LiveStreamSeen> _seenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudFlareLiveService _cloudFlareLiveService;

    public LiveStreamService(
        IRepository<LiveStream> repository,
        IRepository<LiveStreamSeen> seenRepository,
        IUnitOfWork unitOfWork,
        ICloudFlareLiveService cloudFlareLiveService)
    {
        _repository = repository;
        _seenRepository = seenRepository;
        _unitOfWork = unitOfWork;
        _cloudFlareLiveService = cloudFlareLiveService;
    }

    public async Task<List<LiveStreamDto>> GetAllAsync()
    {
        return await _repository.Query()
            .Include(ls => ls.Course)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Teacher)
            .OrderByDescending(ls => ls.ScheduledAt)
            .Select(ls => new LiveStreamDto
            {
                Id = ls.Id,
                Title = ls.Title,
                CourseId = ls.CourseId,
                CourseTitle = ls.Course != null ? ls.Course.Title : null,
                SubjectId = ls.SubjectId,
                SubjectName = ls.Subject.SubjectName,
                TeacherId = ls.TeacherId,
                TeacherName = ls.Teacher.FullName,
                ScheduledAt = ls.ScheduledAt,
                CloudflareStreamId = ls.CloudflareStreamId,
                StreamUrl = ls.StreamUrl,
                Status = ls.Status,
                SeenCount = ls.SeenCount
            })
            .ToListAsync();
    }

    public async Task<List<LiveStreamDto>> GetBySchoolIdAsync(int schoolId)
    {
        return await _repository.Query()
            .Where(ls => ls.SchoolId == schoolId)
            .Include(ls => ls.Course)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Teacher)
            .OrderByDescending(ls => ls.ScheduledAt)
            .Select(ls => new LiveStreamDto
            {
                Id = ls.Id,
                Title = ls.Title,
                CourseId = ls.CourseId,
                CourseTitle = ls.Course != null ? ls.Course.Title : null,
                SubjectId = ls.SubjectId,
                SubjectName = ls.Subject.SubjectName,
                TeacherId = ls.TeacherId,
                TeacherName = ls.Teacher.FullName,
                ScheduledAt = ls.ScheduledAt,
                CloudflareStreamId = ls.CloudflareStreamId,
                StreamUrl = ls.StreamUrl,
                Status = ls.Status,
                SeenCount = ls.SeenCount
            })
            .ToListAsync();
    }

    public async Task<List<LiveStreamDto>> GetAllByCourseAsync(int courseId)
    {
        return await _repository.Query()
            .Include(ls => ls.Course)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Teacher)
            .Where(ls => ls.CourseId == courseId)
            .OrderByDescending(ls => ls.ScheduledAt)
            .Select(ls => new LiveStreamDto
            {
                Id = ls.Id,
                Title = ls.Title,
                CourseId = ls.CourseId,
                CourseTitle = ls.Course != null ? ls.Course.Title : null,
                SubjectId = ls.SubjectId,
                SubjectName = ls.Subject.SubjectName,
                TeacherId = ls.TeacherId,
                TeacherName = ls.Teacher.FullName,
                ScheduledAt = ls.ScheduledAt,
                CloudflareStreamId = ls.CloudflareStreamId,
                StreamUrl = ls.StreamUrl,
                Status = ls.Status,
                SeenCount = ls.SeenCount
            })
            .ToListAsync();
    }

    public async Task<List<LiveStreamDto>> GetByTeacherIdAsync(int teacherId)
    {
        return await _repository.Query()
            .Where(ls => ls.TeacherId == teacherId)
            .Include(ls => ls.Course)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Teacher)
            .OrderByDescending(ls => ls.ScheduledAt)
            .Select(ls => new LiveStreamDto
            {
                Id = ls.Id,
                Title = ls.Title,
                CourseId = ls.CourseId,
                CourseTitle = ls.Course != null ? ls.Course.Title : null,
                SubjectId = ls.SubjectId,
                SubjectName = ls.Subject.SubjectName,
                TeacherId = ls.TeacherId,
                TeacherName = ls.Teacher.FullName,
                ScheduledAt = ls.ScheduledAt,
                CloudflareStreamId = ls.CloudflareStreamId,
                StreamUrl = ls.StreamUrl,
                Status = ls.Status,
                SeenCount = ls.SeenCount
            })
            .ToListAsync();
    }

    public async Task<List<LiveStreamDto>> GetBySubjectIdsAsync(List<int> subjectIds)
    {
        if (subjectIds == null || subjectIds.Count == 0) return new List<LiveStreamDto>();

        return await _repository.Query()
            .Where(ls => subjectIds.Contains(ls.SubjectId))
            .Include(ls => ls.Course)
            .Include(ls => ls.Subject)
            .Include(ls => ls.Teacher)
            .OrderByDescending(ls => ls.ScheduledAt)
            .Select(ls => new LiveStreamDto
            {
                Id = ls.Id,
                Title = ls.Title,
                CourseId = ls.CourseId,
                CourseTitle = ls.Course != null ? ls.Course.Title : null,
                SubjectId = ls.SubjectId,
                SubjectName = ls.Subject.SubjectName,
                TeacherId = ls.TeacherId,
                TeacherName = ls.Teacher.FullName,
                ScheduledAt = ls.ScheduledAt,
                CloudflareStreamId = ls.CloudflareStreamId,
                StreamUrl = ls.StreamUrl,
                Status = ls.Status,
                SeenCount = ls.SeenCount
            })
            .ToListAsync();
    }

    public async Task<LiveStreamDto?> GetByIdAsync(int id)
    {
        var ls = await _repository.Query()
            .Include(x => x.Course)
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (ls == null) return null;

        return new LiveStreamDto
        {
            Id = ls.Id,
            Title = ls.Title,
            CourseId = ls.CourseId,
            CourseTitle = ls.Course?.Title,
            SubjectId = ls.SubjectId,
            SubjectName = ls.Subject.SubjectName,
            TeacherId = ls.TeacherId,
            TeacherName = ls.Teacher.FullName,
            ScheduledAt = ls.ScheduledAt,
            CloudflareStreamId = ls.CloudflareStreamId,
            StreamUrl = ls.StreamUrl,
            Status = ls.Status,
            SeenCount = ls.SeenCount
        };
    }

    public async Task<LiveStreamDto> CreateAsync(CreateLiveStreamDto dto)
    {
        var cfResponse = await _cloudFlareLiveService.CreateLiveInputAsync(dto.Title);

        var entity = new LiveStream
        {
            Title = dto.Title,
            CourseId = dto.CourseId,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            ScheduledAt = dto.ScheduledAt,
            SchoolId = dto.SchoolId,
            Status = LiveStreamStatus.Scheduled,
            CloudflareStreamId = cfResponse?.Result?.Uid,
            StreamUrl = cfResponse?.Result?.Rtmps?.Url
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new LiveStreamDto
        {
            Id = entity.Id,
            Title = entity.Title,
            CourseId = entity.CourseId,
            TeacherId = entity.TeacherId,
            ScheduledAt = entity.ScheduledAt,
            CloudflareStreamId = entity.CloudflareStreamId,
            StreamUrl = entity.StreamUrl,
            RtmpsUrl = cfResponse?.Result?.Rtmps?.Url,
            StreamKey = cfResponse?.Result?.Rtmps?.StreamKey,
            Status = entity.Status
        };
    }

    public async Task UpdateStatusAsync(int id, LiveStreamStatus status)
    {
        var entity = await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException("Live stream not found.");

        if (status == LiveStreamStatus.Live && string.IsNullOrEmpty(entity.CloudflareStreamId))
        {
            var cfResponse = await _cloudFlareLiveService.CreateLiveInputAsync(entity.Title);
            entity.CloudflareStreamId = cfResponse?.Result?.Uid;
            entity.StreamUrl = cfResponse?.Result?.Rtmps?.Url;
        }
        else if (status == LiveStreamStatus.Ended && !string.IsNullOrEmpty(entity.CloudflareStreamId))
        {
            await _cloudFlareLiveService.DeleteLiveInputAsync(entity.CloudflareStreamId);
        }

        entity.Status = status;
        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            if (!string.IsNullOrEmpty(entity.CloudflareStreamId))
            {
                await _cloudFlareLiveService.DeleteLiveInputAsync(entity.CloudflareStreamId);
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkSeenAsync(int liveStreamId, int studentId)
    {
        var alreadySeen = await _seenRepository.Query()
            .AnyAsync(s => s.LiveStreamId == liveStreamId && s.StudentId == studentId && !s.IsDeleted);

        if (alreadySeen) return;

        var seen = new LiveStreamSeen
        {
            LiveStreamId = liveStreamId,
            StudentId = studentId,
            SeenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        await _seenRepository.AddAsync(seen);

        var stream = await _repository.GetByIdAsync(liveStreamId);
        if (stream != null)
        {
            stream.SeenCount += 1;
            _repository.Update(stream);
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
