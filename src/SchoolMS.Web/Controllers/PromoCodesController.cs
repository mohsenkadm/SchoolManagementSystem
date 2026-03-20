using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class PromoCodesController : Controller
{
    private readonly IPromoCodeService _service;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public PromoCodesController(
        IPromoCodeService service,
        IPlatformService platformService,
        IBranchService branchService,
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

    [HasPermission("PromoCodes", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Promo Codes";
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
                : new List<PromoCodeDto>();
        return View(all);
    }

    [HasPermission("PromoCodes", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Promo Code";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("PromoCodes", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PromoCodeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Promo Code", $"Use code {dto.Code} for a discount!", new[] { "Student", "Parent" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("PromoCodes", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Promo Code";
        await LoadCreateViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("PromoCodes", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PromoCodeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("PromoCodes", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PromoCodes.xlsx");
    }

    private async Task LoadCreateViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
    }
}
