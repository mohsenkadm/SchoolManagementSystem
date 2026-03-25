using System.Text.Json;
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
    private readonly IRepository<VideoComment> _commentRepository;
    private readonly IRepository<VideoQuizQuestion> _quizQuestionRepo;
    private readonly IRepository<VideoQuizAnswer> _quizAnswerRepo;
    private readonly IRepository<VideoNote> _noteRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBunnyStreamService _bunnyStreamService;

    public CourseVideoService(
        IRepository<CourseVideo> repository,
        IRepository<Course> courseRepository,
        IRepository<VideoLike> likeRepository,
        IRepository<VideoSeen> seenRepository,
        IRepository<VideoRating> ratingRepository,
        IRepository<VideoComment> commentRepository,
        IRepository<VideoQuizQuestion> quizQuestionRepo,
        IRepository<VideoQuizAnswer> quizAnswerRepo,
        IRepository<VideoNote> noteRepo,
        IUnitOfWork unitOfWork,
        IBunnyStreamService bunnyStreamService)
    {
        _repository = repository;
        _courseRepository = courseRepository;
        _likeRepository = likeRepository;
        _seenRepository = seenRepository;
        _ratingRepository = ratingRepository;
        _commentRepository = commentRepository;
        _quizQuestionRepo = quizQuestionRepo;
        _quizAnswerRepo = quizAnswerRepo;
        _noteRepo = noteRepo;
        _unitOfWork = unitOfWork;
        _bunnyStreamService = bunnyStreamService;
    }

    public async Task<List<CourseVideoDto>> GetAllByCourseAsync(int courseId)
    {
        return await _repository.Query()
            .Include(v => v.Course)
            .Include(v => v.QuizQuestions)
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
                RatingCount = v.RatingCount,
                QuizQuestionCount = v.QuizQuestions.Count(q => !q.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<List<CourseVideoDto>> GetAllByCourseForStudentAsync(int courseId, int studentId)
    {
        var videos = await _repository.Query()
            .Include(v => v.Course)
            .Include(v => v.Seens)
            .Include(v => v.QuizQuestions)
            .Where(v => v.CourseId == courseId && v.IsDeleted==false && v.IsScheduled==false)
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
                RatingCount = v.RatingCount,
                IsWatched = v.Seens.Any(s => s.StudentId == studentId && !s.IsDeleted),
                QuizQuestionCount = v.QuizQuestions.Count(q => !q.IsDeleted)
            })
            .ToListAsync();

        return videos;
    }

    public async Task<List<CourseVideoDto>> GetFreeVideosByCourseAsync(int courseId)
    {
        return await _repository.Query()
            .Include(v => v.Course)
            .Include(v => v.QuizQuestions)
            .Where(v => v.CourseId == courseId && v.IsFreeTrial && !v.IsDeleted && !v.IsScheduled)
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
                RatingCount = v.RatingCount,
                QuizQuestionCount = v.QuizQuestions.Count(q => !q.IsDeleted)
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

        // Save quiz questions if provided during video creation
        if (dto.QuizQuestions != null && dto.QuizQuestions.Count > 0)
        {
            foreach (var q in dto.QuizQuestions)
            {
                var question = new VideoQuizQuestion
                {
                    CourseVideoId = entity.Id,
                    QuestionText = q.QuestionText,
                    Options = JsonSerializer.Serialize(q.Options),
                    CorrectAnswer = q.CorrectAnswer,
                    SortOrder = q.SortOrder,
                    SchoolId = course.SchoolId
                };
                await _quizQuestionRepo.AddAsync(question);
            }
            await _unitOfWork.SaveChangesAsync();
        }

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

    public async Task<List<VideoCommentDto>> GetCommentsByVideoIdAsync(int courseVideoId)
    {
        return await _commentRepository.Query()
            .Where(c => c.CourseVideoId == courseVideoId)
            .Include(c => c.Student)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new VideoCommentDto
            {
                Id = c.Id,
                CourseVideoId = c.CourseVideoId,
                StudentId = c.StudentId,
                StudentName = c.Student.FullName,
                Comment = c.Comment,
                CommentDate = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<VideoCommentDto> AddCommentAsync(CreateVideoCommentDto dto)
    {
        var video = await _repository.GetByIdAsync(dto.CourseVideoId)
            ?? throw new InvalidOperationException("Course video not found.");

        var entity = new VideoComment
        {
            CourseVideoId = dto.CourseVideoId,
            StudentId = dto.StudentId,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            SchoolId = video.SchoolId
        };
        await _commentRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new VideoCommentDto
        {
            Id = entity.Id,
            CourseVideoId = entity.CourseVideoId,
            StudentId = entity.StudentId,
            Comment = entity.Comment,
            CommentDate = entity.CreatedAt
        };
    }

    // ===== Video Quiz Questions =====

    public async Task<List<VideoQuizQuestionDto>> GetQuizQuestionsByVideoAsync(int courseVideoId)
    {
        return await _quizQuestionRepo.Query()
            .Where(q => q.CourseVideoId == courseVideoId)
            .OrderBy(q => q.SortOrder)
            .Select(q => new VideoQuizQuestionDto
            {
                Id = q.Id,
                CourseVideoId = q.CourseVideoId,
                QuestionText = q.QuestionText,
                Options = DeserializeOptions(q.Options),
                CorrectAnswer = q.CorrectAnswer,
                SortOrder = q.SortOrder,
                AnswerCount = q.Answers.Count(a => !a.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<VideoQuizQuestionDto> CreateQuizQuestionAsync(CreateVideoQuizQuestionDto dto)
    {
        var video = await _repository.GetByIdAsync(dto.CourseVideoId)
            ?? throw new InvalidOperationException("Course video not found.");

        var entity = new VideoQuizQuestion
        {
            CourseVideoId = dto.CourseVideoId,
            QuestionText = dto.QuestionText,
            Options = JsonSerializer.Serialize(dto.Options),
            CorrectAnswer = dto.CorrectAnswer,
            SortOrder = dto.SortOrder,
            SchoolId = video.SchoolId
        };
        await _quizQuestionRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new VideoQuizQuestionDto
        {
            Id = entity.Id,
            CourseVideoId = entity.CourseVideoId,
            QuestionText = entity.QuestionText,
            Options = dto.Options,
            CorrectAnswer = entity.CorrectAnswer,
            SortOrder = entity.SortOrder
        };
    }

    public async Task<VideoQuizQuestionDto> UpdateQuizQuestionAsync(VideoQuizQuestionDto dto)
    {
        var entity = await _quizQuestionRepo.GetByIdAsync(dto.Id)
            ?? throw new InvalidOperationException("Quiz question not found.");

        entity.QuestionText = dto.QuestionText;
        entity.Options = JsonSerializer.Serialize(dto.Options);
        entity.CorrectAnswer = dto.CorrectAnswer;
        entity.SortOrder = dto.SortOrder;
        _quizQuestionRepo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return dto;
    }

    public async Task DeleteQuizQuestionAsync(int id)
    {
        var entity = await _quizQuestionRepo.GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _quizQuestionRepo.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    // ===== Video Quiz Answers =====

    public async Task<VideoQuizAnswerDto> SubmitQuizAnswerAsync(SubmitVideoQuizAnswerDto dto)
    {
        var question = await _quizQuestionRepo.GetByIdAsync(dto.VideoQuizQuestionId)
            ?? throw new InvalidOperationException("Quiz question not found.");

        // Check for existing answer — update instead of duplicate
        var existing = await _quizAnswerRepo.Query()
            .FirstOrDefaultAsync(a => a.VideoQuizQuestionId == dto.VideoQuizQuestionId
                && a.StudentId == dto.StudentId && !a.IsDeleted);

        bool isCorrect = string.Equals(dto.SelectedAnswer?.Trim(), question.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);

        if (existing != null)
        {
            existing.SelectedAnswer = dto.SelectedAnswer;
            existing.IsCorrect = isCorrect;
            existing.AnsweredAt = DateTime.UtcNow;
            _quizAnswerRepo.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            return new VideoQuizAnswerDto
            {
                Id = existing.Id,
                VideoQuizQuestionId = existing.VideoQuizQuestionId,
                StudentId = existing.StudentId,
                SelectedAnswer = existing.SelectedAnswer,
                IsCorrect = existing.IsCorrect,
                AnsweredAt = existing.AnsweredAt
            };
        }

        var entity = new VideoQuizAnswer
        {
            VideoQuizQuestionId = dto.VideoQuizQuestionId,
            StudentId = dto.StudentId,
            SelectedAnswer = dto.SelectedAnswer,
            IsCorrect = isCorrect,
            AnsweredAt = DateTime.UtcNow,
            SchoolId = question.SchoolId
        };
        await _quizAnswerRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new VideoQuizAnswerDto
        {
            Id = entity.Id,
            VideoQuizQuestionId = entity.VideoQuizQuestionId,
            StudentId = entity.StudentId,
            SelectedAnswer = entity.SelectedAnswer,
            IsCorrect = entity.IsCorrect,
            AnsweredAt = entity.AnsweredAt
        };
    }

    public async Task<List<VideoQuizAnswerDto>> GetStudentAnswersAsync(int courseVideoId, int studentId)
    {
        return await _quizAnswerRepo.Query()
            .Include(a => a.VideoQuizQuestion)
            .Include(a => a.Student)
            .Where(a => a.VideoQuizQuestion.CourseVideoId == courseVideoId && a.StudentId == studentId)
            .OrderBy(a => a.VideoQuizQuestion.SortOrder)
            .Select(a => new VideoQuizAnswerDto
            {
                Id = a.Id,
                VideoQuizQuestionId = a.VideoQuizQuestionId,
                QuestionText = a.VideoQuizQuestion.QuestionText,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                SelectedAnswer = a.SelectedAnswer,
                IsCorrect = a.IsCorrect,
                AnsweredAt = a.AnsweredAt
            })
            .ToListAsync();
    }

    public async Task<List<VideoQuizAnswerDto>> GetAllAnswersByVideoAsync(int courseVideoId)
    {
        return await _quizAnswerRepo.Query()
            .Include(a => a.VideoQuizQuestion)
            .Include(a => a.Student)
            .Where(a => a.VideoQuizQuestion.CourseVideoId == courseVideoId)
            .OrderBy(a => a.Student.FullName).ThenBy(a => a.VideoQuizQuestion.SortOrder)
            .Select(a => new VideoQuizAnswerDto
            {
                Id = a.Id,
                VideoQuizQuestionId = a.VideoQuizQuestionId,
                QuestionText = a.VideoQuizQuestion.QuestionText,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                SelectedAnswer = a.SelectedAnswer,
                IsCorrect = a.IsCorrect,
                AnsweredAt = a.AnsweredAt
            })
            .ToListAsync();
    }

    // ===== Video Notes =====

    public async Task<VideoNoteDto?> GetNoteAsync(int courseVideoId, int studentId)
    {
        var note = await _noteRepo.Query()
            .FirstOrDefaultAsync(n => n.CourseVideoId == courseVideoId && n.StudentId == studentId);
        if (note == null) return null;
        return new VideoNoteDto
        {
            Id = note.Id,
            CourseVideoId = note.CourseVideoId,
            StudentId = note.StudentId,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };
    }

    public async Task<VideoNoteDto> SaveNoteAsync(SaveVideoNoteDto dto)
    {
        var existing = await _noteRepo.Query()
            .FirstOrDefaultAsync(n => n.CourseVideoId == dto.CourseVideoId && n.StudentId == dto.StudentId && !n.IsDeleted);

        if (existing != null)
        {
            existing.NoteText = dto.NoteText;
            _noteRepo.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return new VideoNoteDto
            {
                Id = existing.Id,
                CourseVideoId = existing.CourseVideoId,
                StudentId = existing.StudentId,
                NoteText = existing.NoteText,
                CreatedAt = existing.CreatedAt,
                UpdatedAt = existing.UpdatedAt
            };
        }

        var video = await _repository.GetByIdAsync(dto.CourseVideoId)
            ?? throw new InvalidOperationException("Course video not found.");

        var entity = new VideoNote
        {
            CourseVideoId = dto.CourseVideoId,
            StudentId = dto.StudentId,
            NoteText = dto.NoteText,
            SchoolId = video.SchoolId
        };
        await _noteRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return new VideoNoteDto
        {
            Id = entity.Id,
            CourseVideoId = entity.CourseVideoId,
            StudentId = entity.StudentId,
            NoteText = entity.NoteText,
            CreatedAt = entity.CreatedAt
        };
    }

    private static List<string> DeserializeOptions(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }
}
