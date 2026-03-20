using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class StudentsController : Controller
{
    private readonly IStudentService _studentService;
    private readonly IBranchService _branchService;
    private readonly IAcademicYearService _academicYearService;
    private readonly IGradeService _gradeService;
    private readonly IDivisionService _divisionService;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubscriptionLimitService _limitService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public StudentsController(
        IStudentService studentService,
        IBranchService branchService,
        IAcademicYearService academicYearService,
        IGradeService gradeService,
        IDivisionService divisionService,
        IClassRoomService classRoomService,
        ISubscriptionLimitService limitService,
        IPlatformService platformService,
        IOneSignalNotificationService pushService)
    {
        _studentService = studentService;
        _branchService = branchService;
        _academicYearService = academicYearService;
        _gradeService = gradeService;
        _divisionService = divisionService;
        _classRoomService = classRoomService;
        _limitService = limitService;
        _platformService = platformService;
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

    [HasPermission("Students", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Students";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = new List<BranchDto>();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        ViewBag.AcademicYears = await _academicYearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.Grades = await _gradeService.GetAllAsync();
        ViewBag.Divisions = await _divisionService.GetAllAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches);
    }

    [HttpPost]
    public async Task<IActionResult> GetData([FromBody] DataTableRequest request)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue && !request.SchoolId.HasValue)
            request.SchoolId = CurrentSchoolId;
        var result = await _studentService.GetDataTableAsync(request);
        return Json(result);
    }

    [HasPermission("Students", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Student";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Students", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStudentDto dto)
    {
        if (!ModelState.IsValid) { await LoadViewBags(); return View(dto); }
        var schoolClaim = User.FindFirst("SchoolId");
        if (schoolClaim != null && int.TryParse(schoolClaim.Value, out var schoolId))
        {
            var (allowed, error) = await _limitService.CanAddStudentAsync(schoolId);
            if (!allowed) { ModelState.AddModelError("", error ?? "Limit reached"); await LoadViewBags(); return View(dto); }
        }
        await _studentService.CreateAsync(dto);
        if (schoolClaim != null && int.TryParse(schoolClaim.Value, out var sid))
            await _pushService.SendToPersonTypesAsync("New Student Registered",
                $"{dto.FullName} has been enrolled",
                new[] { "Staff" }, sid);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Students", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var student = await _studentService.GetByIdAsync(id);
        if (student == null) return NotFound();
        ViewData["Title"] = "Edit Student";
        await LoadViewBags();
        return View("Create", student);
    }

    [HttpPost, HasPermission("Students", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateStudentDto dto)
    {
        if (!ModelState.IsValid) { await LoadViewBags(); return View("Create", dto); }
        await _studentService.UpdateAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Student Updated",
                "A student record has been updated",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Students", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _studentService.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _studentService.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx");
    }

    private async Task LoadViewBags()
    {
        ViewBag.Branches = await _branchService.GetAllAsync();
        ViewBag.AcademicYears = await _academicYearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.Grades = await _gradeService.GetAllAsync();
        ViewBag.Divisions = await _divisionService.GetAllAsync();
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
    }
}
