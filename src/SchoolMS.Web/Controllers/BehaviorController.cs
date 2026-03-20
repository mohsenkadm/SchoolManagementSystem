using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class BehaviorController : Controller
{
    private readonly IStudentBehaviorService _service;
    private readonly IStudentService _studentService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public BehaviorController(IStudentBehaviorService service, IStudentService studentService, IBranchService branchService,
        IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _studentService = studentService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Behavior", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Student Behavior";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin) { ViewBag.Schools = await _platformService.GetAllSchoolsAsync(); ViewBag.Branches = await _branchService.GetAllAsync(); }
        else { ViewBag.Schools = new List<SchoolDto>(); ViewBag.Branches = CurrentSchoolId.HasValue ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value) : new List<BranchDto>(); }
        return View(await _service.GetAllAsync());
    }

    [HasPermission("Behavior", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Behavior Record";
        await LoadViewBags();
        ViewBag.Students = await _studentService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("Behavior", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentBehaviorDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("Behavior Record",
            $"{dto.Title} - {dto.StudentName ?? "Student"}",
            dto.StudentId, "Parent", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Behavior", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id); if (item == null) return NotFound();
        ViewData["Title"] = "Edit Behavior Record";
        await LoadViewBags(item.SchoolId);
        ViewBag.Students = await _studentService.GetAllAsync();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Behavior", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentBehaviorDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToIndividualAsync("Behavior Record Updated",
            $"{dto.Title} - {dto.StudentName ?? "Student"} has been updated",
            dto.StudentId, "Parent", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Behavior", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    private async Task LoadViewBags(int? editSchoolId = null)
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        if (IsSuperAdmin)
            ViewBag.Branches = editSchoolId.HasValue && editSchoolId.Value > 0 ? await _branchService.GetBySchoolIdAsync(editSchoolId.Value) : new List<BranchDto>();
        else
            ViewBag.Branches = CurrentSchoolId.HasValue ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value) : new List<BranchDto>();
    }
}
