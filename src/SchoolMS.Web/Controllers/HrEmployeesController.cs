using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrEmployeesController : Controller
{
    private readonly IHrEmployeeService _service;
    private readonly IHrDepartmentService _deptService;
    private readonly IHrJobTitleService _titleService;
    private readonly IHrJobGradeService _gradeService;
    private readonly IBranchService _branchService;
    private readonly IHrWorkShiftService _shiftService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public HrEmployeesController(IHrEmployeeService service, IHrDepartmentService deptService,
        IHrJobTitleService titleService, IHrJobGradeService gradeService,
        IBranchService branchService, IHrWorkShiftService shiftService, IPlatformService platformService, IOneSignalNotificationService pushService)
    {
        _service = service; _deptService = deptService; _titleService = titleService;
        _gradeService = gradeService; _branchService = branchService; _shiftService = shiftService; _platformService = platformService; _pushService = pushService;
    }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrEmployees", "View")]
    public async Task<IActionResult> Index()
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
            ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
            : await _service.GetAllAsync();
        return View(list);
    }

    [HasPermission("HrEmployees", "View")]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HasPermission("HrEmployees", "Add")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    [HttpPost, HasPermission("HrEmployees", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrEmployeeDto dto)
    {
        await _service.CreateAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("New Employee Added",
                $"{dto.FullName} has been added to HR",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("HrEmployees", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        await PopulateDropdowns();
        return View("Create", item);
    }

    [HttpPost, HasPermission("HrEmployees", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrEmployeeDto dto)
    {
        await _service.UpdateAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Employee Updated",
                $"{dto.FullName} record has been updated",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("HrEmployees", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HasPermission("HrEmployees", "View")]
    public async Task<IActionResult> ByDepartment(int departmentId) => Json(await _service.GetByDepartmentAsync(departmentId));

    [HasPermission("HrEmployees", "View")]
    public async Task<IActionResult> ByBranch(int branchId) => Json(await _service.GetByBranchAsync(branchId));

    private async Task PopulateDropdowns()
    {
        ViewBag.Departments = await _deptService.GetAllAsync();
        ViewBag.JobTitles = await _titleService.GetAllAsync();
        ViewBag.JobGrades = await _gradeService.GetAllAsync();
        ViewBag.Branches = await _branchService.GetAllAsync();
        ViewBag.WorkShifts = await _shiftService.GetAllAsync();
        ViewBag.Employees = await _service.GetAllAsync();
    }
}
