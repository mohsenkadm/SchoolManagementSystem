using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Models;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Dashboard";
        int? branchId = null;
        int? schoolId = null;
        var branchClaim = User.FindFirst("BranchId");
        if (branchClaim != null)
            branchId = int.Parse(branchClaim.Value);
        var schoolClaim = User.FindFirst("SchoolId");
        if (schoolClaim != null)
            schoolId = int.Parse(schoolClaim.Value);

        var dashboard = await _dashboardService.GetDashboardDataAsync(branchId, schoolId);
        return View(dashboard);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
