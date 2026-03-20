using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class TeacherAssignmentsController : Controller
{
    private readonly ITeacherAssignmentService _service;
    private readonly ITeacherService _teacherService;
    private readonly ISubjectService _subjectService;
    private readonly IGradeService _gradeService;
    private readonly IDivisionService _divisionService;
    private readonly IAcademicYearService _yearService;
    private readonly IBranchService _branchService;
    private readonly IClassRoomService _classRoomService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public TeacherAssignmentsController(ITeacherAssignmentService service, ITeacherService teacherService,
        ISubjectService subjectService, IGradeService gradeService, IDivisionService divisionService,
        IAcademicYearService yearService, IBranchService branchService, IClassRoomService classRoomService,
        IPlatformService platformService, IOneSignalNotificationService pushService)
    {
        _service = service; _teacherService = teacherService; _subjectService = subjectService;
        _gradeService = gradeService; _divisionService = divisionService;
        _yearService = yearService; _branchService = branchService; _classRoomService = classRoomService;
        _platformService = platformService; _pushService = pushService;
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

    [HasPermission("TeacherAssignments", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Teacher Assignments";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<TeacherAssignmentDto>();
        return View(all);
    }

    [HasPermission("TeacherAssignments", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Assignment";
        await LoadViewBags();
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View();
    }

    [HttpPost, HasPermission("TeacherAssignments", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TeacherAssignmentDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("New Assignment",
            $"You have been assigned to {dto.SubjectName ?? "a subject"}",
            dto.TeacherId, "Teacher", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("TeacherAssignments", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Assignment";
        await LoadViewBags();
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View("Create", item);
    }

    [HttpPost, HasPermission("TeacherAssignments", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TeacherAssignmentDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToIndividualAsync("Assignment Updated",
            $"Your assignment for {dto.SubjectName ?? "a subject"} has been updated",
            dto.TeacherId, "Teacher", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("TeacherAssignments", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    private async Task LoadViewBags()
    {
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.Branches = await _branchService.GetAllAsync();
    }
}
