using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class AcademicYearsController : Controller
{
    private readonly IAcademicYearService _service;
    private readonly IPlatformService _platformService;

    public AcademicYearsController(IAcademicYearService service, IPlatformService platformService)
    {
        _service = service;
        _platformService = platformService;
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

    [HasPermission("AcademicYears", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "AcademicYears";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();

        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetAllAsync(CurrentSchoolId.Value)
                : new List<AcademicYearDto>();
        return View(all);
    }

    [HasPermission("AcademicYears", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Academic Year";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View();
    }

    [HttpPost, HasPermission("AcademicYears", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AcademicYearDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("AcademicYears", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Academic Year";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        return View("Create", item);
    }

    [HttpPost, HasPermission("AcademicYears", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AcademicYearDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("AcademicYears", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
