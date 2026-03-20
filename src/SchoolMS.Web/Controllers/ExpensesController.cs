using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IExpenseService _service;
    private readonly IExpenseTypeService _typeService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public ExpensesController(IExpenseService service, IExpenseTypeService typeService,
        IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _typeService = typeService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    private async Task SetCommonViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
    }

    [HasPermission("Expenses", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Expenses";
        await SetCommonViewBags();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<ExpenseDto>();
        return View(all);
    }

    [HasPermission("Expenses", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Expense";
        await SetCommonViewBags();
        ViewBag.ExpenseTypes = await _typeService.GetAllAsync();
        ViewBag.Branches = await _branchService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("Expenses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExpenseDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Expense Recorded",
            $"{dto.ExpenseTypeName ?? "Expense"}: {dto.Amount:N2}",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Expenses", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var items = await _service.GetAllAsync();
        var item = items.FirstOrDefault(e => e.Id == id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Expense";
        await SetCommonViewBags();
        ViewBag.ExpenseTypes = await _typeService.GetAllAsync();
        ViewBag.Branches = await _branchService.GetAllAsync();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Expenses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExpenseDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Expense Updated",
            $"{dto.ExpenseTypeName ?? "Expense"}: {dto.Amount:N2} has been updated",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Expenses", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
