using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class AssetsController : Controller
{
    private readonly IAssetService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public AssetsController(IAssetService service, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Assets", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Asset Management";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Categories = await _service.GetAllCategoriesAsync();
        if (IsSuperAdmin) { ViewBag.Schools = await _platformService.GetAllSchoolsAsync(); ViewBag.Branches = await _branchService.GetAllAsync(); }
        else { ViewBag.Schools = new List<SchoolDto>(); ViewBag.Branches = CurrentSchoolId.HasValue ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value) : new List<BranchDto>(); }
        return View(await _service.GetAllAssetsAsync());
    }

    [HasPermission("Assets", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Asset";
        ViewBag.Categories = await _service.GetAllCategoriesAsync();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Assets", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAssetAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Asset Added",
            $"{dto.AssetName} has been registered",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Assets", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetAssetByIdAsync(id); if (item == null) return NotFound();
        ViewData["Title"] = "Edit Asset";
        ViewBag.Categories = await _service.GetAllCategoriesAsync();
        await LoadViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Assets", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AssetDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAssetAsync(dto);
        await _pushService.SendToPersonTypesAsync("Asset Updated",
            $"{dto.AssetName} has been updated",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Assets", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAssetAsync(id); return Ok(); }

    [HttpPost, HasPermission("Assets", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory([FromBody] AssetCategoryDto dto) => Ok(await _service.CreateCategoryAsync(dto));

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
