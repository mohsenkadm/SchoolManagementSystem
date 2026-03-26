using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class HomeController : Controller
{
    private readonly SchoolDbContext _context;

    public HomeController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        // Carousel
        var carousels = await _context.CarouselImages.IgnoreQueryFilters()
            .Where(c => c.SchoolId == schoolId && c.IsActive && !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        // My active subscriptions
        var mySubscriptions = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Where(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted
                && s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow)
            .ToListAsync();

        var subscribedPlanIds = mySubscriptions.Select(s => s.OnlineSubscriptionPlanId).ToHashSet();

        // All published courses for this school
        var courses = await _context.Set<Course>().IgnoreQueryFilters()
            .Include(c => c.Teacher)
            .Include(c => c.Subject)
            .Include(c => c.Videos)
            .Where(c => c.SchoolId == schoolId && c.IsPublished && !c.IsDeleted)
            .ToListAsync();

        // My subscribed courses (via subject)
        var subscribedSubjectIds = mySubscriptions
            .Select(s => s.OnlineSubscriptionPlan.SubjectId).ToHashSet();

        var myCourses = courses.Where(c => subscribedSubjectIds.Contains(c.SubjectId)).ToList();

        // Notifications
        var student = await _context.Students.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);

        var notifications = await _context.Set<Notification>().IgnoreQueryFilters()
            .Where(n => n.SchoolId == schoolId && !n.IsDeleted && n.IsSent &&
                (n.Target == NotificationTarget.All ||
                 (n.Target == NotificationTarget.Individual && n.TargetPersonId == studentId && n.TargetPersonType == PersonType.Student) ||
                 (n.Target == NotificationTarget.Class && student != null && n.TargetClassRoomId == student.ClassRoomId) ||
                 (n.Target == NotificationTarget.Branch && student != null && n.TargetBranchId == student.BranchId)))
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .ToListAsync();

        // Live streams
        var liveStreams = await _context.Set<LiveStream>().IgnoreQueryFilters()
            .Include(l => l.Teacher).Include(l => l.Subject).Include(l => l.Course)
            .Where(l => l.SchoolId == schoolId && !l.IsDeleted
                && (l.Status == LiveStreamStatus.Live || l.Status == LiveStreamStatus.Scheduled))
            .OrderBy(l => l.ScheduledAt)
            .Take(6)
            .ToListAsync();

        ViewBag.Carousels = carousels;
        ViewBag.MyCourses = myCourses;
        ViewBag.AllCourses = courses;
        ViewBag.MySubscriptions = mySubscriptions;
        ViewBag.Notifications = notifications;
        ViewBag.LiveStreams = liveStreams;
        ViewBag.StudentName = HttpContext.Session.GetString("StudentName");

        return View();
    }

    public IActionResult DownloadApp()
    {
        return View();
    }
}
