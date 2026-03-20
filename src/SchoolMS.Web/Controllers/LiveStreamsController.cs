using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class LiveStreamsController : Controller
{
    private readonly ILiveStreamService _service;
    private readonly ICourseService _courseService;
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public LiveStreamsController(
        ILiveStreamService service,
        ICourseService courseService,
        ITeacherService teacherService,
        ISubjectService subjectService,
        IPlatformService platformService,
        IBranchService branchService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _courseService = courseService;
        _teacherService = teacherService;
        _subjectService = subjectService;
        _platformService = platformService;
        _branchService = branchService;
        _pushService = pushService;
    }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    [HasPermission("Courses", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Live Streams";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
        var items = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<LiveStreamDto>();
        return View(items);
    }

    [HasPermission("Courses", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Create Live Stream";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Courses = await _courseService.GetAllAsync();
            ViewBag.Teachers = await _teacherService.GetAllAsync();
            ViewBag.Subjects = await _subjectService.GetAllAsync();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Courses = CurrentSchoolId.HasValue
                ? await _courseService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<CourseDto>();
            ViewBag.Teachers = CurrentSchoolId.HasValue
                ? await _teacherService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<TeacherDto>();
            ViewBag.Subjects = CurrentSchoolId.HasValue
                ? await _subjectService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
        }
        return View();
    }

    [HttpPost, HasPermission("Courses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLiveStreamDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        else if (dto.SchoolId == 0 && dto.CourseId.HasValue)
        {
            var course = await _courseService.GetByIdAsync(dto.CourseId.Value);
            if (course != null)
                dto.SchoolId = course.SchoolId;
        }
        // If CourseId is provided, auto-resolve SubjectId from course
        if (dto.CourseId.HasValue && dto.SubjectId == 0)
        {
            var course = await _courseService.GetByIdAsync(dto.CourseId.Value);
            if (course != null)
                dto.SubjectId = course.SubjectId;
        }
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Live Stream",
            $"{dto.Title} scheduled at {dto.ScheduledAt:g}",
            new[] { "Student", "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetCoursesBySchool(int schoolId)
    {
        var courses = await _courseService.GetBySchoolIdAsync(schoolId);
        return Json(courses.Select(c => new { c.Id, c.Title }));
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachersBySchool(int schoolId)
    {
        var teachers = await _teacherService.GetBySchoolIdAsync(schoolId);
        return Json(teachers.Select(t => new { t.Id, t.FullName }));
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjectsBySchool(int schoolId)
    {
        var subjects = await _subjectService.GetBySchoolIdAsync(schoolId);
        return Json(subjects.Select(s => new { s.Id, s.SubjectName }));
    }

    [HttpPost, HasPermission("Courses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LiveStreamStatus status)
    {
        await _service.UpdateStatusAsync(id, status);
        return Ok();
    }

    [HttpDelete("{id}"), HasPermission("Courses", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
