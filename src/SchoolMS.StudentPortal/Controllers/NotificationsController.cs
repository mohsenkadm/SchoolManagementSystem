using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class NotificationsController : Controller
{
    private readonly SchoolDbContext _context;

    public NotificationsController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var student = await _context.Students.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);

        var notifications = await _context.Set<Notification>().IgnoreQueryFilters()
            .Where(n => n.SchoolId == schoolId && !n.IsDeleted && n.IsSent &&
                (n.Target == NotificationTarget.All ||
                 (n.Target == NotificationTarget.Individual && n.TargetPersonId == studentId && n.TargetPersonType == PersonType.Student) ||
                 (n.Target == NotificationTarget.Class && student != null && n.TargetClassRoomId == student.ClassRoomId) ||
                 (n.Target == NotificationTarget.Branch && student != null && n.TargetBranchId == student.BranchId)))
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(notifications);
    }
}
