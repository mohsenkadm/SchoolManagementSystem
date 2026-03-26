using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class CoursesController : Controller
{
    private readonly SchoolDbContext _context;

    public CoursesController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;

        var courses = await _context.Set<Course>().IgnoreQueryFilters()
            .Include(c => c.Teacher).Include(c => c.Subject)
            .Include(c => c.Videos.Where(v => !v.IsDeleted))
            .Where(c => c.SchoolId == schoolId && c.IsPublished && !c.IsDeleted)
            .ToListAsync();

        var mySubscriptions = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .Include(s => s.OnlineSubscriptionPlan)
            .Where(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted
                && s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow)
            .ToListAsync();

        var subscribedSubjectIds = mySubscriptions
            .Select(s => s.OnlineSubscriptionPlan.SubjectId).ToHashSet();

        // Subscriber count per subject
        var subjectSubscriberCounts = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .Where(s => s.SchoolId == schoolId && !s.IsDeleted
                && s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow)
            .GroupBy(s => s.OnlineSubscriptionPlan.SubjectId)
            .Select(g => new { SubjectId = g.Key, Count = g.Select(x => x.StudentId).Distinct().Count() })
            .ToDictionaryAsync(x => x.SubjectId, x => x.Count);

        ViewBag.Courses = courses;
        ViewBag.SubscribedSubjectIds = subscribedSubjectIds;
        ViewBag.SubjectSubscriberCounts = subjectSubscriberCounts;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;

        var course = await _context.Set<Course>().IgnoreQueryFilters()
            .Include(c => c.Teacher).Include(c => c.Subject)
            .Include(c => c.Videos
                .Where(v => !v.IsDeleted && (!v.IsScheduled || (v.ScheduledPublishAt != null && v.ScheduledPublishAt <= DateTime.UtcNow)))
                .OrderBy(v => v.SortOrder))
                .ThenInclude(v => v.Comments.Where(cm => !cm.IsDeleted))
                    .ThenInclude(cm => cm.Student)
            .Include(c => c.Videos).ThenInclude(v => v.Ratings)
            .Include(c => c.Videos).ThenInclude(v => v.Seens)
            .Include(c => c.Videos).ThenInclude(v => v.Notes.Where(n => n.StudentId == studentId && !n.IsDeleted))
            .Include(c => c.Videos).ThenInclude(v => v.QuizQuestions.Where(q => !q.IsDeleted).OrderBy(q => q.SortOrder))
                .ThenInclude(q => q.Answers.Where(a => a.StudentId == studentId))
            .FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && !c.IsDeleted);

        if (course == null) return NotFound();

        // Check subscription
        var hasAccess = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .AnyAsync(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted
                && s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow
                && s.OnlineSubscriptionPlan.SubjectId == course.SubjectId);

        ViewBag.HasAccess = hasAccess;
        ViewBag.StudentId = studentId;
        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> MarkSeen(int videoId)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var exists = await _context.Set<VideoSeen>().IgnoreQueryFilters()
            .AnyAsync(s => s.CourseVideoId == videoId && s.StudentId == studentId && s.SchoolId == schoolId);

        if (!exists)
        {
            _context.Set<VideoSeen>().Add(new VideoSeen
            {
                CourseVideoId = videoId, StudentId = studentId, SchoolId = schoolId,
                SeenAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
            });

            var video = await _context.Set<CourseVideo>().IgnoreQueryFilters()
                .FirstOrDefaultAsync(v => v.Id == videoId && v.SchoolId == schoolId);
            if (video != null) video.SeenCount++;

            await _context.SaveChangesAsync();
        }
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(int videoId, string comment)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        _context.Set<VideoComment>().Add(new VideoComment
        {
            CourseVideoId = videoId, StudentId = studentId, SchoolId = schoolId,
            Comment = comment, CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
        });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddNote(int videoId, string noteText)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        _context.Set<VideoNote>().Add(new VideoNote
        {
            CourseVideoId = videoId, StudentId = studentId, SchoolId = schoolId,
            NoteText = noteText, CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
        });
        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Rate(int videoId, int rating)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var existing = await _context.Set<VideoRating>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.CourseVideoId == videoId && r.StudentId == studentId && r.SchoolId == schoolId);

        if (existing != null)
            existing.Rating = rating;
        else
        {
            _context.Set<VideoRating>().Add(new VideoRating
            {
                CourseVideoId = videoId, StudentId = studentId, SchoolId = schoolId,
                Rating = rating, CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
            });
        }

        await _context.SaveChangesAsync();

        // Update average
        var video = await _context.Set<CourseVideo>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.Id == videoId && v.SchoolId == schoolId);
        if (video != null)
        {
            var ratings = await _context.Set<VideoRating>().IgnoreQueryFilters()
                .Where(r => r.CourseVideoId == videoId && r.SchoolId == schoolId && !r.IsDeleted)
                .ToListAsync();
            video.AverageRating = ratings.Count > 0 ? ratings.Average(r => r.Rating) : 0;
            video.RatingCount = ratings.Count;
            await _context.SaveChangesAsync();
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> SubmitQuiz(int questionId, string answer)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var question = await _context.Set<VideoQuizQuestion>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(q => q.Id == questionId && q.SchoolId == schoolId);
        if (question == null) return Json(new { success = false });

        var isCorrect = question.CorrectAnswer == answer;

        var existing = await _context.Set<VideoQuizAnswer>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(a => a.VideoQuizQuestionId == questionId && a.StudentId == studentId && a.SchoolId == schoolId);

        if (existing != null)
        {
            existing.SelectedAnswer = answer;
            existing.IsCorrect = isCorrect;
            existing.AnsweredAt = DateTime.UtcNow;
        }
        else
        {
            _context.Set<VideoQuizAnswer>().Add(new VideoQuizAnswer
            {
                VideoQuizQuestionId = questionId, StudentId = studentId, SchoolId = schoolId,
                SelectedAnswer = answer, IsCorrect = isCorrect, AnsweredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
            });
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true, isCorrect });
    }

    public async Task<IActionResult> QuizResults(int id)
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;

        var course = await _context.Set<Course>().IgnoreQueryFilters()
            .Include(c => c.Teacher).Include(c => c.Subject)
            .Include(c => c.Videos.Where(v => !v.IsDeleted).OrderBy(v => v.SortOrder))
                .ThenInclude(v => v.QuizQuestions.Where(q => !q.IsDeleted).OrderBy(q => q.SortOrder))
                    .ThenInclude(q => q.Answers.Where(a => a.StudentId == studentId))
            .FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && !c.IsDeleted);

        if (course == null) return NotFound();

        ViewBag.StudentId = studentId;
        return View(course);
    }
}
