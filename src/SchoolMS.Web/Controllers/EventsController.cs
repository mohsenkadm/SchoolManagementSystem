using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class EventsController : Controller
{
    private readonly ISchoolEventService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public EventsController(ISchoolEventService service, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Events", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "School Events";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin) { ViewBag.Schools = await _platformService.GetAllSchoolsAsync(); ViewBag.Branches = await _branchService.GetAllAsync(); }
        else { ViewBag.Schools = new List<SchoolDto>(); ViewBag.Branches = CurrentSchoolId.HasValue ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value) : new List<BranchDto>(); }
        return View(await _service.GetAllAsync());
    }

    [HasPermission("Events", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Event";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Events", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SchoolEventDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToSchoolAsync("New Event",
            $"{dto.Title} - {dto.StartDate:d}", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Events", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id); if (item == null) return NotFound();
        ViewData["Title"] = "Edit Event";
        await LoadViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Events", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SchoolEventDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToSchoolAsync("Event Updated",
            $"{dto.Title} - {dto.StartDate:d} has been updated", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Events", "Delete")]
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
