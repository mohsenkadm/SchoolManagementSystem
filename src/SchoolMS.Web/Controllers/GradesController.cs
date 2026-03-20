using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class GradesController : Controller
{
    private readonly IGradeService _gradeService;
    private readonly IDivisionService _divisionService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public GradesController(IGradeService gradeService, IDivisionService divisionService,
        IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    {
        _gradeService = gradeService;
        _divisionService = divisionService;
        _branchService = branchService;
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

    [HasPermission("Grades", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Grades";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = new List<BranchDto>();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : await _branchService.GetAllAsync();
        }
        var all = IsSuperAdmin
            ? await _gradeService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _gradeService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<GradeDto>();
        return View(all);
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    [HttpGet]
    public async Task<IActionResult> GetDivisionsBySchool(int schoolId)
    {
        var divisions = await _divisionService.GetBySchoolIdAsync(schoolId);
        return Json(divisions.Select(d => new { d.Id, d.DivisionName }));
    }

    [HasPermission("Grades", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Grade";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("Grades", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GradeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _gradeService.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Grade Added", $"{dto.GradeName} has been created", new[] { "Staff", "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Grades", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _gradeService.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Grade";
        await LoadCreateViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Grades", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(GradeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _gradeService.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Grade Updated", $"{dto.GradeName} has been updated", new[] { "Staff", "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Grades", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _gradeService.DeleteAsync(id); return Ok(); }

    private async Task LoadCreateViewBags(int? editSchoolId = null)
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();

        if (IsSuperAdmin)
        {
            // When editing, pre-load divisions/branches for the item's school
            if (editSchoolId.HasValue && editSchoolId.Value > 0)
            {
                ViewBag.Divisions = await _divisionService.GetBySchoolIdAsync(editSchoolId.Value);
                ViewBag.Branches = await _branchService.GetBySchoolIdAsync(editSchoolId.Value);
            }
            else
            {
                ViewBag.Divisions = new List<DivisionDto>();
                ViewBag.Branches = new List<BranchDto>();
            }
        }
        else
        {
            ViewBag.Divisions = CurrentSchoolId.HasValue
                ? await _divisionService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<DivisionDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
    }
}
