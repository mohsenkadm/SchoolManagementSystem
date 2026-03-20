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
    public async Task<ActionResult<List<HrPerformanceCycleDto>>> GetCycles()
        => Ok(await _service.GetCyclesAsync());

    [HttpPost("cycles")]
    public async Task<ActionResult<HrPerformanceCycleDto>> CreateCycle([FromBody] HrPerformanceCycleDto dto)
        => Ok(await _service.CreateCycleAsync(dto));

    [HttpPut("cycles")]
    public async Task<ActionResult<HrPerformanceCycleDto>> UpdateCycle([FromBody] HrPerformanceCycleDto dto)
        => Ok(await _service.UpdateCycleAsync(dto));

    // Criteria
    [HttpGet("criteria")]
    public async Task<ActionResult<List<HrPerformanceCriteriaDto>>> GetCriteria()
        => Ok(await _service.GetCriteriaAsync());

    [HttpPost("criteria")]
    public async Task<ActionResult<HrPerformanceCriteriaDto>> CreateCriteria([FromBody] HrPerformanceCriteriaDto dto)
        => Ok(await _service.CreateCriteriaAsync(dto));

    [HttpPut("criteria")]
    public async Task<ActionResult<HrPerformanceCriteriaDto>> UpdateCriteria([FromBody] HrPerformanceCriteriaDto dto)
        => Ok(await _service.UpdateCriteriaAsync(dto));

    [HttpDelete("criteria/{id}")]
    public async Task<IActionResult> DeleteCriteria(int id) { await _service.DeleteCriteriaAsync(id); return Ok(); }

    // Reviews
    [HttpGet("reviews")]
    public async Task<ActionResult<List<HrPerformanceReviewDto>>> GetReviews(
        [FromQuery] int? cycleId, [FromQuery] int? employeeId)
        => Ok(await _service.GetReviewsAsync(cycleId, employeeId));

    [HttpGet("reviews/{id}")]
    public async Task<ActionResult<HrPerformanceReviewDto>> GetReview(int id)
    {
        var item = await _service.GetReviewByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("reviews")]
    public async Task<ActionResult<HrPerformanceReviewDto>> CreateReview([FromBody] HrPerformanceReviewDto dto)
        => Ok(await _service.CreateReviewAsync(dto));

    [HttpPut("reviews")]
    public async Task<ActionResult<HrPerformanceReviewDto>> UpdateReview([FromBody] HrPerformanceReviewDto dto)
        => Ok(await _service.UpdateReviewAsync(dto));

    [HttpPost("reviews/{id}/complete")]
    public async Task<IActionResult> CompleteReview(int id)
    {
        await _service.CompleteReviewAsync(id);
        return Ok();
    }

    // KPIs
    [HttpGet("kpis")]
    public async Task<ActionResult<List<HrKpiDto>>> GetKpis([FromQuery] int? employeeId)
        => Ok(await _service.GetKpisAsync(employeeId));

    [HttpPost("kpis")]
    public async Task<ActionResult<HrKpiDto>> CreateKpi([FromBody] HrKpiDto dto)
        => Ok(await _service.CreateKpiAsync(dto));

    [HttpPut("kpis")]
    public async Task<ActionResult<HrKpiDto>> UpdateKpi([FromBody] HrKpiDto dto)
        => Ok(await _service.UpdateKpiAsync(dto));

    [HttpDelete("kpis/{id}")]
    public async Task<IActionResult> DeleteKpi(int id) { await _service.DeleteKpiAsync(id); return Ok(); }
}
