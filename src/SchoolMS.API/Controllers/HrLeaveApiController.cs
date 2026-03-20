using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة إجازات الموظفين والأرصدة والعطل الرسمية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/leave")]
[Authorize]
public class HrLeaveApiController : ControllerBase
{
    private readonly IHrLeaveService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrLeaveApiController(IHrLeaveService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    // Leave Requests
    [HttpGet("requests")]
    public async Task<ActionResult<List<HrLeaveRequestDto>>> GetAllRequests([FromQuery] HrLeaveStatus? status)
        => Ok(await _service.GetAllRequestsAsync(status));

    [HttpPost("requests")]
    public async Task<ActionResult<HrLeaveRequestDto>> CreateRequest(int schoolId, [FromBody] HrLeaveRequestDto dto)
    { var r = await _service.CreateRequestAsync(dto); await _pushService.SendToPersonTypesAsync("HR Leave Request", "A new leave request has been submitted", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPost("requests/{id}/approve-manager")]
    public async Task<IActionResult> ApproveByManager(int schoolId, int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveByManagerAsync(id, approvedBy);
        await _pushService.SendToPersonTypesAsync("Leave Approved by Manager", "A leave request has been approved by manager", new[] { "Staff" }, schoolId);
        return Ok();
    }

    [HttpPost("requests/{id}/approve-hr")]
    public async Task<IActionResult> ApproveByHr(int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveByHrAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("requests/{id}/reject")]
    public async Task<IActionResult> Reject(int schoolId, int id, [FromQuery] string rejectedBy, [FromQuery] string reason)
    {
        await _service.RejectAsync(id, rejectedBy, reason);
        await _pushService.SendToPersonTypesAsync("Leave Rejected", "A leave request has been rejected", new[] { "Staff" }, schoolId);
        return Ok();
    }

    [HttpPost("requests/{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelAsync(id);
        return Ok();
    }

    // Leave Types
    [HttpGet("types")]
    public async Task<ActionResult<List<HrLeaveTypeDto>>> GetLeaveTypes()
        => Ok(await _service.GetLeaveTypesAsync());

    [HttpPost("types")]
    public async Task<ActionResult<HrLeaveTypeDto>> CreateLeaveType([FromBody] HrLeaveTypeDto dto)
        => Ok(await _service.CreateLeaveTypeAsync(dto));

    [HttpPut("types")]
    public async Task<ActionResult<HrLeaveTypeDto>> UpdateLeaveType([FromBody] HrLeaveTypeDto dto)
        => Ok(await _service.UpdateLeaveTypeAsync(dto));

    [HttpDelete("types/{id}")]
    public async Task<IActionResult> DeleteLeaveType(int id) { await _service.DeleteLeaveTypeAsync(id); return Ok(); }

    // Leave Balances
    [HttpGet("balances/{employeeId}")]
    public async Task<ActionResult<List<HrLeaveBalanceDto>>> GetBalances(int employeeId)
        => Ok(await _service.GetBalancesAsync(employeeId));

    [HttpGet("balances/all")]
    public async Task<ActionResult<List<HrLeaveBalanceDto>>> GetAllBalances([FromQuery] int year)
        => Ok(await _service.GetAllBalancesAsync(year));

    [HttpPost("balances/initialize")]
    public async Task<IActionResult> InitializeBalances([FromQuery] int employeeId, [FromQuery] int year)
    {
        await _service.InitializeBalancesAsync(employeeId, year);
        return Ok();
    }

    // Holidays
    [HttpGet("holidays")]
    public async Task<ActionResult<List<HrHolidayDto>>> GetHolidays([FromQuery] int? year)
        => Ok(await _service.GetHolidaysAsync(year));

    [HttpPost("holidays")]
    public async Task<ActionResult<HrHolidayDto>> CreateHoliday([FromBody] HrHolidayDto dto)
        => Ok(await _service.CreateHolidayAsync(dto));

    [HttpPut("holidays")]
    public async Task<ActionResult<HrHolidayDto>> UpdateHoliday([FromBody] HrHolidayDto dto)
        => Ok(await _service.UpdateHolidayAsync(dto));

    [HttpDelete("holidays/{id}")]
    public async Task<IActionResult> DeleteHoliday(int id) { await _service.DeleteHolidayAsync(id); return Ok(); }
}
