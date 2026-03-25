using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrDisciplinaryController : Controller
{
    private readonly IHrDisciplinaryService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public HrDisciplinaryController(IHrDisciplinaryService service, IHrEmployeeService empService, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrDisciplinary", "View")]
    public async Task<IActionResult> Index(int? employeeId)
    {
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
        var list = CurrentSchoolId.HasValue
            ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value, employeeId)
            : await _service.GetAllAsync(employeeId);
        return View(list);
    }

    [HasPermission("HrDisciplinary", "View")]
    public async Task<IActionResult> Details(int id) => View(await _service.GetByIdAsync(id));

    [HasPermission("HrDisciplinary", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = await _empService.GetAllAsync();
        ViewBag.ViolationTypes = await _service.GetViolationTypesAsync();
        return View();
    }

    [HttpPost, HasPermission("HrDisciplinary", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrDisciplinaryActionDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Disciplinary Action", "A new disciplinary action has been recorded", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrDisciplinary", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrDisciplinaryActionDto dto) { await _service.UpdateAsync(dto); return RedirectToAction(nameof(Index)); }

    // Violation Types
    [HasPermission("HrDisciplinary", "View")]
    public async Task<IActionResult> ViolationTypes() => View(await _service.GetViolationTypesAsync());

    [HttpPost, HasPermission("HrDisciplinary", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateViolationType(HrViolationTypeDto dto) { await _service.CreateViolationTypeAsync(dto); return RedirectToAction(nameof(ViolationTypes)); }

    [HttpPost, HasPermission("HrDisciplinary", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateViolationType(HrViolationTypeDto dto) { await _service.UpdateViolationTypeAsync(dto); return RedirectToAction(nameof(ViolationTypes)); }

    [HttpDelete("violation-type/{id}"), HasPermission("HrDisciplinary", "Delete")]
    public async Task<IActionResult> DeleteViolationType(int id) { await _service.DeleteViolationTypeAsync(id); return Ok(); }
}
