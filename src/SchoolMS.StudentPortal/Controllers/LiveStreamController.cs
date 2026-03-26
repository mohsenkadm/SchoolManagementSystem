using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class LiveStreamController : Controller
{
    private readonly SchoolDbContext _context;

    public LiveStreamController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var streams = await _context.Set<LiveStream>().IgnoreQueryFilters()
            .Include(l => l.Teacher).Include(l => l.Subject).Include(l => l.Course)
            .Where(l => l.SchoolId == schoolId && !l.IsDeleted)
            .OrderByDescending(l => l.ScheduledAt)
            .ToListAsync();

        return View(streams);
    }

    public async Task<IActionResult> Watch(int id)
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;

        var stream = await _context.Set<LiveStream>().IgnoreQueryFilters()
            .Include(l => l.Teacher).Include(l => l.Subject).Include(l => l.Course)
            .Include(l => l.Comments.OrderByDescending(c => c.SentAt).Take(50))
            .FirstOrDefaultAsync(l => l.Id == id && l.SchoolId == schoolId && !l.IsDeleted);

        if (stream == null) return NotFound();

        // Mark seen
        var seen = await _context.Set<LiveStreamSeen>().IgnoreQueryFilters()
            .AnyAsync(s => s.LiveStreamId == id && s.StudentId == studentId && s.SchoolId == schoolId);

        if (!seen)
        {
            _context.Set<LiveStreamSeen>().Add(new LiveStreamSeen
            {
                LiveStreamId = id, StudentId = studentId, SchoolId = schoolId,
                SeenAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, CreatedBy = studentId.ToString()
            });
            stream.SeenCount++;
            await _context.SaveChangesAsync();
        }

        ViewBag.StudentId = studentId;
        return View(stream);
    }
}
