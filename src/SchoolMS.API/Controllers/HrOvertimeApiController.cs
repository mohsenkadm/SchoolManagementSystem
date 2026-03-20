using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة طلبات العمل الإضافي
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/overtime")]
[Authorize]
public class HrOvertimeApiController : ControllerBase
{
    private readonly IHrOvertimeService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrOvertimeApiController(IHrOvertimeService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrOvertimeRequestDto>>> GetAll(int schoolId, [FromQuery] OvertimeStatus? status)
        => Ok(await _service.GetAllAsync(status));

    [HttpPost]
    public async Task<ActionResult<HrOvertimeRequestDto>> Create(int schoolId, [FromBody] HrOvertimeRequestDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("Overtime Request", "A new overtime request has been submitted", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int schoolId, int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(int schoolId, int id, [FromQuery] string rejectedBy, [FromQuery] string reason)
    {
        await _service.RejectAsync(id, rejectedBy, reason);
        return Ok();
    }
}
