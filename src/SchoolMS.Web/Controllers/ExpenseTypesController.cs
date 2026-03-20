using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ExpenseTypesController : Controller
{
    private readonly IExpenseTypeService _service;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public ExpenseTypesController(IExpenseTypeService service, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    [HasPermission("Expenses", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Expense Types";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<ExpenseTypeDto>();
        return View(all);
    }

    [HasPermission("Expenses", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Expense Type";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View();
    }

    [HttpPost, HasPermission("Expenses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseTypeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Expense Type", $"{dto.TypeName} has been added", new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Expenses", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Expense Type";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Expenses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExpenseTypeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Expenses", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
