using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة تقييم أداء الموظفين ومؤشرات الأداء
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/performance")]
[Authorize]
public class HrPerformanceApiController : ControllerBase
{
    private readonly IHrPerformanceService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrPerformanceApiController(IHrPerformanceService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;
                
    // Reviews — returns only the logged-in employee's reviews
    [HttpGet("reviews")]
    public async Task<ActionResult<List<HrPerformanceReviewDto>>> GetReviews(int schoolId, [FromQuery] int? cycleId)
        => Ok(await _service.GetReviewsBySchoolIdAsync(schoolId, cycleId, GetEmployeeIdFromToken()));

    // KPIs — returns only the logged-in employee's KPIs
    [HttpGet("kpis")]
    public async Task<ActionResult<List<HrKpiDto>>> GetKpis(int schoolId)
        => Ok(await _service.GetKpisBySchoolIdAsync(schoolId, GetEmployeeIdFromToken()));

}
