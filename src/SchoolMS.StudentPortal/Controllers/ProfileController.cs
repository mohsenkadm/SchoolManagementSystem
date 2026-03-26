using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class ProfileController : Controller
{
    private readonly SchoolDbContext _context;

    public ProfileController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var student = await _context.Students.IgnoreQueryFilters()
            .Include(s => s.Branch).Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId && !s.IsDeleted);

        if (student == null) return NotFound();

        // Stats
        var subCount = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .CountAsync(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted
                && s.Status == SubscriptionStatus.Approved);

        var seenCount = await _context.Set<VideoSeen>().IgnoreQueryFilters()
            .CountAsync(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted);

        ViewBag.SubCount = subCount;
        ViewBag.SeenCount = seenCount;
        return View(student);
    }
}
