using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;

    public ReportsController(IAnalyticsService analyticsService, IPlatformService platformService, IBranchService branchService)
    {
        _analyticsService = analyticsService;
        _platformService = platformService;
        _branchService = branchService;
    }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Reports", "View")]
    public async Task<IActionResult> Analytics(int? schoolId, int? branchId)
    {
        ViewData["Title"] = "Analytics";
        ViewBag.IsSuperAdmin = IsSuperAdmin;

        int? filterSchoolId;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            filterSchoolId = schoolId;
            ViewBag.Branches = filterSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(filterSchoolId.Value)
                : new List<BranchDto>();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            filterSchoolId = CurrentSchoolId;
            ViewBag.Branches = filterSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(filterSchoolId.Value)
                : new List<BranchDto>();
        }

        ViewBag.SelectedSchoolId = filterSchoolId;
        ViewBag.SelectedBranchId = branchId;

        return View(await _analyticsService.GetAnalyticsAsync(filterSchoolId, branchId));
    }

    [HasPermission("Reports", "View")]
    public async Task<IActionResult> ExportAnalyticsExcel(int? schoolId, int? branchId)
    {
        int? filterSchoolId = IsSuperAdmin ? schoolId : CurrentSchoolId;
        var bytes = await _analyticsService.ExportAnalyticsToExcelAsync(filterSchoolId, branchId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SystemAnalytics.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }
}
