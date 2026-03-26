using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة نهاية الخدمة
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/end-of-service")]
[Authorize]
public class HrEndOfServiceApiController : ControllerBase
{
    private readonly IHrEndOfServiceService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrEndOfServiceApiController(IHrEndOfServiceService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;

    [HttpGet]
    public async Task<ActionResult<List<HrEndOfServiceDto>>> GetAll(int schoolId)
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HrEndOfServiceDto>> GetById(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("calculate")]
    public async Task<ActionResult<HrEndOfServiceDto>> Calculate(
        int schoolId, [FromQuery] int employeeId, [FromQuery] EndOfServiceType type, [FromQuery] DateTime effectiveDate)
        => Ok(await _service.CalculateSettlementAsync(employeeId, type, effectiveDate));

    [HttpPost]
    public async Task<ActionResult<HrEndOfServiceDto>> Create(int schoolId, [FromBody] HrEndOfServiceDto dto)
    {
        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("End of Service",
            "An end of service request has been created",
            new[] { "Staff" }, schoolId);
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int schoolId, int id)
    {
        await _service.ApproveAsync(id, User.Identity?.Name ?? "");
        return Ok();
    }

    [HttpPost("{id}/settle")]
    public async Task<IActionResult> MarkSettled(int schoolId, int id, [FromBody] SettleRequestDto dto)
    {
        await _service.MarkSettledAsync(id, dto.PaymentMethod, dto.PaymentReference);
        return Ok();
    }
}

public class SettleRequestDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentReference { get; set; } = string.Empty;
}
