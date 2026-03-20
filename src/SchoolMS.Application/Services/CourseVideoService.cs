using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class CourseVideoService : ICourseVideoService
{
    private readonly IRepository<CourseVideo> _repository;
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<VideoLike> _likeRepository;
    private readonly IRepository<VideoSeen> _seenRepository;
    private readonly IRepository<VideoRating> _ratingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyStreamService _bunnyStreamService;

    public CourseVideoService(
        IRepository<CourseVideo> repository,
        IRepository<Course> courseRepository,
        IRepository<VideoLike> likeRepository,
        IRepository<VideoSeen> seenRepository,
        IRepository<VideoRating> ratingRepository,
        IUnitOfWork unitOfWork,
        IBunnyStreamService bunnyStreamService)
    {
        _repository = repository;
        _courseRepository = courseRepository;
        _likeRepository = likeRepository;
        _seenRepository = seenRepository;
        _ratingRepository = ratingRepository;
        _unitOfWork = unitOfWork;
        _bunnyStreamService = bunnyStreamService;
    }

    public async Task<List<CourseVideoDto>> GetAllByCourseAsync(int courseId)
    {
        return await _repository.Query()
            .Include(v => v.Course)
            .Where(v => v.CourseId == courseId)
            .OrderBy(v => v.SortOrder)
            .Select(v => new CourseVideoDto
            {
                Id = v.Id,
                CourseId = v.CourseId,
                CourseTitle = v.Course.Title,
                Title = v.Title,
                Description = v.Description,
                BunnyStreamVideoId = v.BunnyStreamVideoId,
                VideoUrl = v.VideoUrl,
                ThumbnailUrl = v.ThumbnailUrl,
                SortOrder = v.SortOrder,
                IsFreeTrial = v.IsFreeTrial,
                ViewCount = v.ViewCount,
                Duration = v.Duration,
                IsScheduled = v.IsScheduled,
                ScheduledPublishAt = v.ScheduledPublishAt,
                FileSizeBytes = v.FileSizeBytes,
                SeenCount = v.SeenCount,
                LikeCount = v.LikeCount,
                AverageRating = v.AverageRating,
                RatingCount = v.RatingCount
            })
            .ToListAsync();
    }

    public async Task<CourseVideoDto?> GetByIdAsync(int id)
    {
        var v = await _repository.Query()
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (v == null) return null;

        return new CourseVideoDto
        {
            Id = v.Id,
            CourseId = v.CourseId,
            CourseTitle = v.Course.Title,
            Title = v.Title,
            Description = v.Description,
            BunnyStreamVideoId = v.BunnyStreamVideoId,
            VideoUrl = v.VideoUrl,
            ThumbnailUrl = v.ThumbnailUrl,
            SortOrder = v.SortOrder,
            IsFreeTrial = v.IsFreeTrial,
            ViewCount = v.ViewCount,
            Duration = v.Duration,
            IsScheduled = v.IsScheduled,
            ScheduledPublishAt = v.ScheduledPublishAt,
            FileSizeBytes = v.FileSizeBytes,
            SeenCount = v.SeenCount,
            LikeCount = v.LikeCount,
            AverageRating = v.AverageRating,
            RatingCount = v.RatingCount
        };
    }

    public async Task<CourseVideoDto> CreateAsync(CreateCourseVideoDto dto)
    {
        // Resolve SchoolId from parent Course so tenant validation passes (e.g. SuperAdmin has no SchoolId claim)
        var course = await _courseRepository.GetByIdAsync(dto.CourseId)
            ?? throw new InvalidOperationException("Course not found.");

        var entity = new CourseVideo
        {
            CourseId = dto.CourseId,
            Title = dto.Title,
            Description = dto.Description,
            SortOrder = dto.SortOrder,
            IsFreeTrial = dto.IsFreeTrial,
            IsScheduled = dto.IsScheduled,
            ScheduledPublishAt = dto.ScheduledPublishAt,
            SchoolId = course.SchoolId
        };

        await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new CourseVideoDto
        {
            Id = entity.Id,
            CourseId = entity.CourseId,
            Title = entity.Title,
            Description = entity.Description,
            SortOrder = entity.SortOrder,
            IsFreeTrial = entity.IsFreeTrial
        };
    }

    public async Task<CourseVideoDto> UpdateAsync(CourseVideoDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Course video not found.");

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.SortOrder = dto.SortOrder;
        entity.IsFreeTrial = dto.IsFreeTrial;
        entity.IsScheduled = dto.IsScheduled;
        entity.ScheduledPublishAt = dto.ScheduledPublishAt;

        if (!string.IsNullOrEmpty(dto.BunnyStreamVideoId))
            entity.BunnyStreamVideoId = dto.BunnyStreamVideoId;
        if (!string.IsNullOrEmpty(dto.VideoUrl))
            entity.VideoUrl = dto.VideoUrl;
        if (!string.IsNullOrEmpty(dto.ThumbnailUrl))
            entity.ThumbnailUrl = dto.ThumbnailUrl;
        if (dto.FileSizeBytes > 0)
            entity.FileSizeBytes = dto.FileSizeBytes;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity != null)
        {
            if (!string.IsNullOrEmpty(entity.BunnyStreamVideoId))
            {
                await _bunnyStreamService.DeleteVideoAsync(entity.BunnyStreamVideoId);
            }

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _repository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ToggleLikeAsync(int courseVideoId, int studentId)
    {
        var existing = await _likeRepository.Query()
            .FirstOrDefaultAsync(l => l.CourseVideoId == courseVideoId && l.StudentId == studentId && !l.IsDeleted);

        var video = await _repository.GetByIdAsync(courseVideoId)
            ?? throw new InvalidOperationException("Course video not found.");

        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            _likeRepository.Update(existing);
            video.LikeCount = Math.Max(0, video.LikeCount - 1);
            _repository.Update(video);
            await _unitOfWork.SaveChangesAsync();
            return false;
        }

        var like = new VideoLike
        {
            CourseVideoId = courseVideoId,
            StudentId = studentId,
            CreatedAt = DateTime.UtcNow
        };
        await _likeRepository.AddAsync(like);
        video.LikeCount += 1;
        _repository.Update(video);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task MarkSeenAsync(int courseVideoId, int studentId)
    {
        var alreadySeen = await _seenRepository.Query()
            .AnyAsync(s => s.CourseVideoId == courseVideoId && s.StudentId == studentId && !s.IsDeleted);

        if (alreadySeen) return;

        var seen = new VideoSeen
        {
            CourseVideoId = courseVideoId,
            StudentId = studentId,
            SeenAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        await _seenRepository.AddAsync(seen);

        var video = await _repository.GetByIdAsync(courseVideoId);
        if (video != null)
        {
            video.SeenCount += 1;
            _repository.Update(video);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<double> RateVideoAsync(int courseVideoId, int studentId, int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        var existing = await _ratingRepository.Query()
            .FirstOrDefaultAsync(r => r.CourseVideoId == courseVideoId && r.StudentId == studentId && !r.IsDeleted);

        if (existing != null)
        {
            existing.Rating = rating;
            existing.UpdatedAt = DateTime.UtcNow;
            _ratingRepository.Update(existing);
        }
        else
        {
            var entity = new VideoRating
            {
                CourseVideoId = courseVideoId,
                StudentId = studentId,
                Rating = rating,
                CreatedAt = DateTime.UtcNow
            };
            await _ratingRepository.AddAsync(entity);
        }

        await _unitOfWork.SaveChangesAsync();

        // Recalculate average from all active ratings
        var ratings = await _ratingRepository.Query()
            .Where(r => r.CourseVideoId == courseVideoId && !r.IsDeleted)
            .ToListAsync();

        var video = await _repository.GetByIdAsync(courseVideoId)
            ?? throw new InvalidOperationException("Course video not found.");

        video.RatingCount = ratings.Count;
        video.AverageRating = ratings.Count > 0 ? Math.Round(ratings.Average(r => r.Rating), 1) : 0;
        _repository.Update(video);
        await _unitOfWork.SaveChangesAsync();

        return video.AverageRating;
    }
}
