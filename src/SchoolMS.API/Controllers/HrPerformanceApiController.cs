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

    // Cycles
    [HttpGet("cycles")]
    public async Task<ActionResult<List<HrPerformanceCycleDto>>> GetCycles(int schoolId)
        => Ok(await _service.GetCyclesBySchoolIdAsync(schoolId));


    // Criteria
    [HttpGet("criteria")]
    public async Task<ActionResult<List<HrPerformanceCriteriaDto>>> GetCriteria()
        => Ok(await _service.GetCriteriaAsync());

    // Reviews
    [HttpGet("reviews")]
    public async Task<ActionResult<List<HrPerformanceReviewDto>>> GetReviews(int schoolId,
        [FromQuery] int? cycleId, [FromQuery] int? employeeId)
        => Ok(await _service.GetReviewsBySchoolIdAsync(schoolId, cycleId, employeeId));


    // KPIs
    [HttpGet("kpis")]
    public async Task<ActionResult<List<HrKpiDto>>> GetKpis(int schoolId, [FromQuery] int? employeeId)
        => Ok(await _service.GetKpisBySchoolIdAsync(schoolId, employeeId));
                           
}
