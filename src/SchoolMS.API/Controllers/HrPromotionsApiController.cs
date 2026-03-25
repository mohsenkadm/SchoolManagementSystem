using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الترقيات والسجل الوظيفي
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/promotions")]
[Authorize]
public class HrPromotionsApiController : ControllerBase
{
    private readonly IHrPromotionService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrPromotionsApiController(IHrPromotionService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrPromotionDto>>> GetAll(int schoolId, [FromQuery] HrPromotionStatus? status)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, status));

    [HttpGet("career-history/{employeeId}")]
    public async Task<ActionResult<List<HrCareerHistoryDto>>> GetCareerHistory(int employeeId)
        => Ok(await _service.GetCareerHistoryAsync(employeeId));
}
