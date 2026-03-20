using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrPerformanceController : Controller
{
    private readonly IHrPerformanceService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IOneSignalNotificationService _pushService;

    public HrPerformanceController(IHrPerformanceService service, IHrEmployeeService empService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    // Cycles
    [HasPermission("HrPerformance", "View")]
    public async Task<IActionResult> Cycles() => View(await _service.GetCyclesAsync());

    [HttpPost, HasPermission("HrPerformance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCycle(HrPerformanceCycleDto dto) { await _service.CreateCycleAsync(dto); return RedirectToAction(nameof(Cycles)); }

    [HttpPost, HasPermission("HrPerformance", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCycle(HrPerformanceCycleDto dto) { await _service.UpdateCycleAsync(dto); return RedirectToAction(nameof(Cycles)); }

    // Criteria
    [HasPermission("HrPerformance", "View")]
    public async Task<IActionResult> Criteria() => View(await _service.GetCriteriaAsync());

    [HttpPost, HasPermission("HrPerformance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCriteria(HrPerformanceCriteriaDto dto) { await _service.CreateCriteriaAsync(dto); return RedirectToAction(nameof(Criteria)); }

    [HttpPost, HasPermission("HrPerformance", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCriteria(HrPerformanceCriteriaDto dto) { await _service.UpdateCriteriaAsync(dto); return RedirectToAction(nameof(Criteria)); }

    [HttpDelete("criteria/{id}"), HasPermission("HrPerformance", "Delete")]
    public async Task<IActionResult> DeleteCriteria(int id) { await _service.DeleteCriteriaAsync(id); return Ok(); }

    // Reviews
    [HasPermission("HrPerformance", "View")]
    public async Task<IActionResult> Reviews(int? cycleId, int? employeeId) => View(await _service.GetReviewsAsync(cycleId, employeeId));

    [HasPermission("HrPerformance", "View")]
    public async Task<IActionResult> ReviewDetails(int id) => View(await _service.GetReviewByIdAsync(id));

    [HasPermission("HrPerformance", "Add")]
    public async Task<IActionResult> CreateReview()
    {
        ViewBag.Employees = await _empService.GetAllAsync();
        ViewBag.Cycles = await _service.GetCyclesAsync();
        return View();
    }

    [HttpPost, HasPermission("HrPerformance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateReview(HrPerformanceReviewDto dto) { await _service.CreateReviewAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Performance Review", "A new performance review has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Reviews)); }

    [HttpPost, HasPermission("HrPerformance", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateReview(HrPerformanceReviewDto dto) { await _service.UpdateReviewAsync(dto); return RedirectToAction(nameof(Reviews)); }

    [HttpPost, HasPermission("HrPerformance", "Edit")]
    public async Task<IActionResult> CompleteReview(int id) { await _service.CompleteReviewAsync(id); return RedirectToAction(nameof(Reviews)); }

    // KPIs
    [HasPermission("HrPerformance", "View")]
    public async Task<IActionResult> Kpis(int? employeeId) => View(await _service.GetKpisAsync(employeeId));

    [HttpPost, HasPermission("HrPerformance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKpi(HrKpiDto dto) { await _service.CreateKpiAsync(dto); return RedirectToAction(nameof(Kpis)); }

    [HttpPost, HasPermission("HrPerformance", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateKpi(HrKpiDto dto) { await _service.UpdateKpiAsync(dto); return RedirectToAction(nameof(Kpis)); }

    [HttpDelete("kpi/{id}"), HasPermission("HrPerformance", "Delete")]
    public async Task<IActionResult> DeleteKpi(int id) { await _service.DeleteKpiAsync(id); return Ok(); }
}
