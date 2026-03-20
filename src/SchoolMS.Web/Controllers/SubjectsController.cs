using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class SubjectsController : Controller
{
    private readonly ISubjectService _service;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public SubjectsController(ISubjectService service, IPlatformService platformService, IBranchService branchService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
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

    [HasPermission("Subjects", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Subjects";
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
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
        return View(all);
    }

    [HasPermission("Subjects", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Subject";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("Subjects", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubjectDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Subject Added",
            $"{dto.SubjectName} has been added",
            new[] { "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Subjects", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Subject";
        await LoadCreateViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Subjects", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubjectDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Subject Updated",
            $"{dto.SubjectName} has been updated",
            new[] { "Teacher" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Subjects", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    private async Task LoadCreateViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
    }
}
