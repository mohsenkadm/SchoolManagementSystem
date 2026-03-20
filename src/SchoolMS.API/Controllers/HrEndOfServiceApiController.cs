using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة نهاية الخدمة والمخالصة
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/end-of-service")]
[Authorize]
public class HrEndOfServiceApiController : ControllerBase
{
    private readonly IHrEndOfServiceService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrEndOfServiceApiController(IHrEndOfServiceService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrEndOfServiceDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HrEndOfServiceDto>> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrEndOfServiceDto>> Create(int schoolId, [FromBody] HrEndOfServiceDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("End of Service", "An end of service request has been created", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPost("calculate")]
    public async Task<ActionResult<HrEndOfServiceDto>> Calculate(
        [FromQuery] int employeeId, [FromQuery] EndOfServiceType type, [FromQuery] DateTime effectiveDate)
        => Ok(await _service.CalculateSettlementAsync(employeeId, type, effectiveDate));

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("{id}/settle")]
    public async Task<IActionResult> MarkSettled(int id, [FromQuery] string paymentMethod, [FromQuery] string paymentReference)
    {
        await _service.MarkSettledAsync(id, paymentMethod, paymentReference);
        return Ok();
    }
}
