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
    public async Task<ActionResult<List<HrPromotionDto>>> GetAll([FromQuery] HrPromotionStatus? status)
        => Ok(await _service.GetAllAsync(status));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrPromotionDto>> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrPromotionDto>> Create(int schoolId, [FromBody] HrPromotionDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("HR Promotion", "A new promotion has been created", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int id, [FromQuery] string rejectedBy, [FromQuery] string reason)
    {
        await _service.RejectAsync(id, rejectedBy, reason);
        return Ok();
    }

    [HttpGet("career-history/{employeeId}")]
    public async Task<ActionResult<List<HrCareerHistoryDto>>> GetCareerHistory(int employeeId)
        => Ok(await _service.GetCareerHistoryAsync(employeeId));
}
