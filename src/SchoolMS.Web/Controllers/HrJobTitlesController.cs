using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrJobTitlesController : Controller
{
    private readonly IHrJobTitleService _service;
    private readonly IHrDepartmentService _deptService;
    private readonly IOneSignalNotificationService _pushService;

    public HrJobTitlesController(IHrJobTitleService service, IHrDepartmentService deptService, IOneSignalNotificationService pushService)
    {
        _service = service;
        _deptService = deptService;
        _pushService = pushService;
    }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrJobTitles", "View")]
    public async Task<IActionResult> Index() => View(await _service.GetAllAsync());

    [HasPermission("HrJobTitles", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Departments = await _deptService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("HrJobTitles", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrJobTitleDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("New Job Title", $"{dto.TitleName} has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrJobTitles", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewBag.Departments = await _deptService.GetAllAsync();
        return View("Create", item);
    }

    [HttpPost, HasPermission("HrJobTitles", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrJobTitleDto dto) { await _service.UpdateAsync(dto); return RedirectToAction(nameof(Index)); }

    [HttpDelete("{id}"), HasPermission("HrJobTitles", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
