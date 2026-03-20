using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrOvertimeController : Controller
{
    private readonly IHrOvertimeService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IOneSignalNotificationService _pushService;
    public HrOvertimeController(IHrOvertimeService service, IHrEmployeeService empService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrOvertime", "View")]
    public async Task<IActionResult> Index(OvertimeStatus? status) => View(await _service.GetAllAsync(status));

    [HasPermission("HrOvertime", "Add")]
    public async Task<IActionResult> Create() { ViewBag.Employees = await _empService.GetAllAsync(); return View(); }

    [HttpPost, HasPermission("HrOvertime", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrOvertimeRequestDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Overtime Request", "A new overtime request has been submitted", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrOvertime", "Edit")]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveAsync(id, User.Identity?.Name ?? "");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, HasPermission("HrOvertime", "Edit")]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        await _service.RejectAsync(id, User.Identity?.Name ?? "", reason);
        return RedirectToAction(nameof(Index));
    }
}
