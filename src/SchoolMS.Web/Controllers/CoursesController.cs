using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Application.Settings;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireOnlinePlatform]
public class CoursesController : Controller
{
    private readonly ICourseService _service;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IWebHostEnvironment _env;
    private readonly IOneSignalNotificationService _pushService;

    public CoursesController(
        ICourseService service,
        ISubjectService subjectService,
        ITeacherService teacherService,
        IPlatformService platformService,
        IBranchService branchService,
        IWebHostEnvironment env,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _subjectService = subjectService;
        _teacherService = teacherService;
        _platformService = platformService;
        _branchService = branchService;
        _env = env;
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
        ViewData["Title"] = "Courses";
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
                : new List<CourseDto>();
        return View(items);
    }

    [HasPermission("Courses", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Course";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();
        ViewBag.Subjects = IsSuperAdmin
            ? await _subjectService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _subjectService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("Courses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseDto dto, IFormFile? ThumbnailImageFile, IFormFile? BackgroundImageFile)
    {
        dto.ThumbnailImage = await SaveImageAsync(ThumbnailImageFile) ?? dto.ThumbnailImage;
        dto.BackgroundImage = await SaveImageAsync(BackgroundImageFile) ?? dto.BackgroundImage;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Course Available",
            $"{dto.Title} is now available",
            new[] { "Student", "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Courses", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Course";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();
        ViewBag.Subjects = IsSuperAdmin
            ? await _subjectService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _subjectService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Courses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CourseDto dto, IFormFile? ThumbnailImageFile, IFormFile? BackgroundImageFile)
    {
        dto.ThumbnailImage = await SaveImageAsync(ThumbnailImageFile) ?? dto.ThumbnailImage;
        dto.BackgroundImage = await SaveImageAsync(BackgroundImageFile) ?? dto.BackgroundImage;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Course Updated",
            $"{dto.Title} has been updated",
            new[] { "Student", "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id) => Json(await _service.GetByIdAsync(id));

    [HttpDelete("{id}"), HasPermission("Courses", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _service.GetByIdAsync(id);
        if (course != null)
        {
            DeleteImageFile(course.ThumbnailImage);
            DeleteImageFile(course.BackgroundImage);
            await _service.DeleteAsync(id);
        }
        return Ok();
    }

    private void DeleteImageFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;

        var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "courses");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return AppUrlSettings.BuildWebUrl($"/uploads/courses/{fileName}");
    }
}
