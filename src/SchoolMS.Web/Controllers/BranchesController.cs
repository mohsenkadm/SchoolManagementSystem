using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class BranchesController : Controller
{
    private readonly IBranchService _service;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public BranchesController(IBranchService service, IPlatformService platformService, IOneSignalNotificationService pushService)
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

    [HasPermission("Branches", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Branches";
        ViewBag.IsSuperAdmin = IsSuperAdmin;

        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            return View(await _service.GetAllAsync());
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            var items = CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
            return View(items);
        }
    }

    [HasPermission("Branches", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Branch";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin
            ? await _platformService.GetAllSchoolsAsync()
            : new List<SchoolDto>();
        return View();
    }

    [HttpPost, HasPermission("Branches", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BranchDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToSchoolAsync("New Branch Added", $"{dto.Name} branch has been created", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Branches", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Branch";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin
            ? await _platformService.GetAllSchoolsAsync()
            : new List<SchoolDto>();
        return View("Create", item);
    }

    [HttpPost, HasPermission("Branches", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BranchDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToSchoolAsync("Branch Updated", $"{dto.Name} branch has been updated", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Branches", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
