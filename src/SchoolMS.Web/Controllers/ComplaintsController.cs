using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ComplaintsController : Controller
{
    private readonly IComplaintService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public ComplaintsController(IComplaintService service, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Complaints", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Complaints";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin) { ViewBag.Schools = await _platformService.GetAllSchoolsAsync(); }
        else { ViewBag.Schools = new List<SchoolDto>(); }
        return View(await _service.GetAllAsync());
    }

    [HasPermission("Complaints", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Complaint";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View();
    }

    [HttpPost, HasPermission("Complaints", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComplaintDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Complaint",
            $"{dto.Subject} - {dto.Category}",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Complaints", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id); if (item == null) return NotFound();
        ViewData["Title"] = "Edit Complaint";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Complaints", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ComplaintDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Complaint Updated",
            $"{dto.Subject} - Status: {dto.Status}",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Complaints", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
