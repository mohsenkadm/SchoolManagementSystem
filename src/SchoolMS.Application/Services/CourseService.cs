using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _repository;
    private readonly IRepository<StudentSubscription> _subscriptionRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IRepository<Course> repository, IRepository<StudentSubscription> subscriptionRepo, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _subscriptionRepo = subscriptionRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        return await _repository.Query()
            .Include(c => c.Subject)
            .Include(c => c.Teacher)
            .Include(c => c.Videos)
            .Include(c => c.LiveStreams)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.SubjectName,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FullName,
                ThumbnailImage = c.ThumbnailImage,
                BackgroundImage = c.BackgroundImage,
                IsPublished = c.IsPublished,
                VideoCount = c.Videos.Count(v => !v.IsDeleted),
                LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
                SchoolId = c.SchoolId
            })
            .ToListAsync();
    }

    public async Task<List<CourseDto>> GetBySchoolIdAsync(int schoolId)
    {
        return await _repository.Query()
            .Where(c => c.SchoolId == schoolId)
            .Include(c => c.Subject)
            .Include(c => c.Teacher)
            .Include(c => c.Videos)
            .Include(c => c.LiveStreams)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.SubjectName,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FullName,
                ThumbnailImage = c.ThumbnailImage,
                BackgroundImage = c.BackgroundImage,
                IsPublished = c.IsPublished,
                VideoCount = c.Videos.Count(v => !v.IsDeleted),
                LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
                SchoolId = c.SchoolId
            })
            .ToListAsync();
    }

    public async Task<List<CourseDto>> GetByTeacherIdAsync(int teacherId)
    {
        return await _repository.Query()
            .Where(c => c.TeacherId == teacherId)
            .Include(c => c.Subject)
            .Include(c => c.Teacher)
            .Include(c => c.Videos)
            .Include(c => c.LiveStreams)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.SubjectName,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FullName,
                ThumbnailImage = c.ThumbnailImage,
                BackgroundImage = c.BackgroundImage,
                IsPublished = c.IsPublished,
                VideoCount = c.Videos.Count(v => !v.IsDeleted),
                LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
                SchoolId = c.SchoolId
            })
            .ToListAsync();
    }

    public async Task<List<CourseDto>> GetBySubjectIdsAsync(List<int> subjectIds)
    {
        if (subjectIds == null || subjectIds.Count == 0) return new List<CourseDto>();

        return await _repository.Query()
            .Where(c => subjectIds.Contains(c.SubjectId) && c.IsPublished)
            .Include(c => c.Subject)
            .Include(c => c.Teacher)
            .Include(c => c.Videos)
            .Include(c => c.LiveStreams)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.SubjectName,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FullName,
                ThumbnailImage = c.ThumbnailImage,
                BackgroundImage = c.BackgroundImage,
                IsPublished = c.IsPublished,
                VideoCount = c.Videos.Count(v => !v.IsDeleted),
                LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
                SchoolId = c.SchoolId
            })
            .ToListAsync();
    }

    public async Task<List<CourseDto>> GetTopCoursesAsync(int schoolId, int count = 10)
    {
        var subscriberCounts = await _subscriptionRepo.Query()
            .Where(s => s.SchoolId == schoolId
                && s.Status == Domain.Enums.SubscriptionStatus.Approved
                && s.OnlineSubscriptionPlan.SubscriptionType == Domain.Enums.SubscriptionType.Course)
            .Include(s => s.OnlineSubscriptionPlan)
            .GroupBy(s => s.OnlineSubscriptionPlan.SubjectId)
            .Select(g => new { SubjectId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.SubjectId, g => g.Count);

        var courses = await _repository.Query()
            .Where(c => c.SchoolId == schoolId && c.IsPublished)
            .Include(c => c.Subject)
            .Include(c => c.Teacher)
            .Include(c => c.Videos)
            .Include(c => c.LiveStreams)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                SubjectId = c.SubjectId,
                SubjectName = c.Subject.SubjectName,
                TeacherId = c.TeacherId,
                TeacherName = c.Teacher.FullName,
                ThumbnailImage = c.ThumbnailImage,
                BackgroundImage = c.BackgroundImage,
                IsPublished = c.IsPublished,
                VideoCount = c.Videos.Count(v => !v.IsDeleted),
                LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
                SchoolId = c.SchoolId
            })
            .ToListAsync();

        foreach (var course in courses)
            course.SubscriberCount = subscriberCounts.GetValueOrDefault(course.SubjectId);

        return courses.OrderByDescending(c => c.SubscriberCount).Take(count).ToList();
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var c = await _repository.Query()
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .Include(x => x.Videos)
            .Include(x => x.LiveStreams)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c == null) return null;

        return new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            SubjectId = c.SubjectId,
            SubjectName = c.Subject.SubjectName,
            TeacherId = c.TeacherId,
            TeacherName = c.Teacher.FullName,
            ThumbnailImage = c.ThumbnailImage,
            BackgroundImage = c.BackgroundImage,
            IsPublished = c.IsPublished,
            VideoCount = c.Videos.Count(v => !v.IsDeleted),
            LiveStreamCount = c.LiveStreams.Count(ls => !ls.IsDeleted),
            SchoolId = c.SchoolId
        };
    }

    public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
    {
        var entity = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            ThumbnailImage = dto.ThumbnailImage,
            BackgroundImage = dto.BackgroundImage,
            IsPublished = dto.IsPublished,
            SchoolId = dto.SchoolId
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new CourseDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            SubjectId = entity.SubjectId,
            TeacherId = entity.TeacherId,
            ThumbnailImage = entity.ThumbnailImage,
            BackgroundImage = entity.BackgroundImage,
            IsPublished = entity.IsPublished,
            SchoolId = entity.SchoolId
        };
    }

    public async Task<CourseDto> UpdateAsync(CourseDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Course not found.");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.SubjectId = dto.SubjectId;
        entity.TeacherId = dto.TeacherId;
        entity.ThumbnailImage = dto.ThumbnailImage;
        entity.BackgroundImage = dto.BackgroundImage;
        entity.IsPublished = dto.IsPublished;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
