using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrPromotionsController : Controller
{
    private readonly IHrPromotionService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IHrDepartmentService _deptService;
    private readonly IHrJobTitleService _titleService;
    private readonly IHrJobGradeService _gradeService;
    private readonly IOneSignalNotificationService _pushService;

    public HrPromotionsController(IHrPromotionService service, IHrEmployeeService empService,
        IHrDepartmentService deptService, IHrJobTitleService titleService, IHrJobGradeService gradeService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _empService = empService; _deptService = deptService;
        _titleService = titleService; _gradeService = gradeService; _pushService = pushService;
    }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrPromotions", "View")]
    public async Task<IActionResult> Index(HrPromotionStatus? status) => View(await _service.GetAllAsync(status));

    [HasPermission("HrPromotions", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = await _empService.GetAllAsync();
        ViewBag.Departments = await _deptService.GetAllAsync();
        ViewBag.JobTitles = await _titleService.GetAllAsync();
        ViewBag.JobGrades = await _gradeService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("HrPromotions", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrPromotionDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("HR Promotion", "A new promotion has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrPromotions", "Edit")]
    public async Task<IActionResult> Approve(int id) { await _service.ApproveAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrPromotions", "Edit")]
    public async Task<IActionResult> Reject(int id, string reason) { await _service.RejectAsync(id, User.Identity?.Name ?? "", reason); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrPromotions", "View")]
    public async Task<IActionResult> CareerHistory(int employeeId) => View(await _service.GetCareerHistoryAsync(employeeId));
}
