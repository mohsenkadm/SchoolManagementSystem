using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrWorkShiftsController : Controller
{
    private readonly IHrWorkShiftService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrWorkShiftsController(IHrWorkShiftService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrWorkShifts", "View")]
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [HasPermission("HrWorkShifts", "Add")]
    public IActionResult Create() => View();

    [HttpPost, HasPermission("HrWorkShifts", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrWorkShiftDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("New Work Shift", $"{dto.ShiftName} has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrWorkShifts", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id); if (item == null) return NotFound();
        return View("Create", item);
    }

    [HttpPost, HasPermission("HrWorkShifts", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrWorkShiftDto dto) { await _service.UpdateAsync(dto); return RedirectToAction(nameof(Index)); }

    [HttpDelete("{id}"), HasPermission("HrWorkShifts", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
