using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class HomeworkController : Controller
{
    private readonly IHomeworkService _service;
    private readonly ITeacherService _teacherService;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IWebHostEnvironment _env;
    private readonly IOneSignalNotificationService _pushService;

    public HomeworkController(IHomeworkService service, ITeacherService teacherService,
        IClassRoomService classRoomService, ISubjectService subjectService, IAcademicYearService yearService,
        IPlatformService platformService, IBranchService branchService, IWebHostEnvironment env,
        IOneSignalNotificationService pushService)
    {
        _service = service; _teacherService = teacherService; _classRoomService = classRoomService;
        _subjectService = subjectService; _yearService = yearService;
        _platformService = platformService; _branchService = branchService; _env = env;
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

    [HasPermission("Homework", "View")]
    public async Task<IActionResult> Index(int? academicYearId = null)
    {
        ViewData["Title"] = "Homework";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Branches = IsSuperAdmin
            ? await _branchService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        ViewBag.AcademicYears = IsSuperAdmin
            ? await _yearService.GetAllAsync()
            : await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.SelectedAcademicYearId = academicYearId;

        var data = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value, academicYearId: academicYearId)
                : new List<HomeworkDto>();
        return View(data);
    }

    [HasPermission("Homework", "Add")]
    public async Task<IActionResult> Create() { ViewData["Title"] = "Add Homework"; await LoadViewBags(); return View(); }

    [HttpPost, HasPermission("Homework", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HomeworkDto dto, List<IFormFile>? attachments)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        var result = await _service.CreateAsync(dto);
        if (attachments != null && attachments.Count > 0)
            await SaveAttachments(result.Id, attachments);
        await _pushService.SendToClassRoomAsync("New Homework",
            $"{dto.Title} - Due: {dto.DueDate:d}",
            new[] { "Parent", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Homework", "Edit")]
    public async Task<IActionResult> Edit(int id) { var item = await _service.GetByIdAsync(id); if (item == null) return NotFound(); ViewData["Title"] = "Edit Homework"; await LoadViewBags(); return View("Create", item); }

    [HttpPost, HasPermission("Homework", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HomeworkDto dto, List<IFormFile>? attachments)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        if (attachments != null && attachments.Count > 0)
            await SaveAttachments(dto.Id, attachments);
        await _pushService.SendToClassRoomAsync("Homework Updated",
            $"{dto.Title} - Due: {dto.DueDate:d} has been updated",
            new[] { "Parent", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Homework", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> GetAttachments(int homeworkId)
    {
        var hw = await _service.GetByIdAsync(homeworkId);
        return Json(hw?.Attachments ?? new List<HomeworkAttachmentDto>());
    }

    private async Task SaveAttachments(int homeworkId, List<IFormFile> files)
    {
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "homework", homeworkId.ToString());
        Directory.CreateDirectory(uploadDir);
        var attachments = new List<HomeworkAttachmentDto>();
        foreach (var file in files)
        {
            if (file.Length == 0) continue;
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            attachments.Add(new HomeworkAttachmentDto
            {
                HomeworkId = homeworkId,
                FileName = file.FileName,
                FileUrl = $"/uploads/homework/{homeworkId}/{fileName}",
                FileType = file.ContentType
            });
        }
        if (attachments.Count > 0)
            await _service.AddAttachmentsAsync(homeworkId, attachments);
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    private async Task LoadViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Branches = IsSuperAdmin
            ? new List<BranchDto>()
            : CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
    }
}
