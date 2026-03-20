using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class TransportController : Controller
{
    private readonly ITransportService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public TransportController(ITransportService service, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Transport", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Transport Routes";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin) { ViewBag.Schools = await _platformService.GetAllSchoolsAsync(); ViewBag.Branches = await _branchService.GetAllAsync(); }
        else { ViewBag.Schools = new List<SchoolDto>(); ViewBag.Branches = CurrentSchoolId.HasValue ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value) : new List<BranchDto>(); }
        return View(await _service.GetAllRoutesAsync());
    }

    [HasPermission("Transport", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Route";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Transport", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransportRouteDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateRouteAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Transport Route",
            $"Route: {dto.RouteName} - Bus: {dto.BusNumber}",
            new[] { "Parent", "Student" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Transport", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetRouteByIdAsync(id); if (item == null) return NotFound();
        ViewData["Title"] = "Edit Route";
        await LoadViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Transport", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TransportRouteDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateRouteAsync(dto);
        await _pushService.SendToPersonTypesAsync("Transport Route Updated",
            $"Route: {dto.RouteName} has been updated",
            new[] { "Parent", "Student" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Transport", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteRouteAsync(id); return Ok(); }

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
