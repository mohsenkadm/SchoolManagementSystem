using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class DivisionsController : Controller
{
    private readonly IDivisionService _service;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public DivisionsController(IDivisionService service, IPlatformService platformService, IOneSignalNotificationService pushService)
    {
        _service = service;
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

    [HasPermission("Divisions", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Divisions";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<DivisionDto>();
        return View(all);
    }

    [HasPermission("Divisions", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Division";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("Divisions", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DivisionDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Division Added", $"{dto.DivisionName} has been created", new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Divisions", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Division";
        await LoadCreateViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Divisions", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DivisionDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Division Updated", $"{dto.DivisionName} has been updated", new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Divisions", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    private async Task LoadCreateViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
    }
}
