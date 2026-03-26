using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class TeachersController : Controller
{
    private readonly SchoolDbContext _context;

    public TeachersController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var teachers = await _context.Teachers.IgnoreQueryFilters()
            .Where(t => t.SchoolId == schoolId && !t.IsDeleted)
            .ToListAsync();

        return View(teachers);
    }

    public async Task<IActionResult> Details(int id)
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var teacher = await _context.Teachers.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && t.SchoolId == schoolId && !t.IsDeleted);

        if (teacher == null) return NotFound();

        var courses = await _context.Set<Course>().IgnoreQueryFilters()
            .Include(c => c.Subject).Include(c => c.Videos)
            .Where(c => c.TeacherId == id && c.SchoolId == schoolId && c.IsPublished && !c.IsDeleted)
            .ToListAsync();

        ViewBag.Courses = courses;
        return View(teacher);
    }
}
