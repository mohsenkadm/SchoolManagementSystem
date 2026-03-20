using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrContractsController : Controller
{
    private readonly IHrContractService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IOneSignalNotificationService _pushService;
    public HrContractsController(IHrContractService service, IHrEmployeeService empService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrContracts", "View")]
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [HasPermission("HrContracts", "View")]
    public async Task<IActionResult> ByEmployee(int employeeId) => Json(await _service.GetByEmployeeAsync(employeeId));

    [HasPermission("HrContracts", "Add")]
    public async Task<IActionResult> Create() { ViewBag.Employees = await _empService.GetAllAsync(); return View(); }

    [HttpPost, HasPermission("HrContracts", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrEmployeeContractDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("New Contract Created", "A new employee contract has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrContracts", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id); if (item == null) return NotFound();
        ViewBag.Employees = await _empService.GetAllAsync(); return View("Create", item);
    }

    [HttpPost, HasPermission("HrContracts", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrEmployeeContractDto dto) { await _service.UpdateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Contract Updated", "An employee contract has been updated", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpDelete("{id}"), HasPermission("HrContracts", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
