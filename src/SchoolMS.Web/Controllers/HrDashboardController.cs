using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrDashboardController : Controller
{
    private readonly IHrDashboardService _service;
    public HrDashboardController(IHrDashboardService service) => _service = service;

    [HasPermission("HrDashboard", "View")]
    public async Task<IActionResult> Index(int? branchId) => View(await _service.GetDashboardAsync(branchId));
}
