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
    public async Task<ActionResult<List<HrLeaveRequestDto>>> GetAllRequests(int schoolId, [FromQuery] HrLeaveStatus? status)
        => Ok(await _service.GetRequestsBySchoolIdAsync(schoolId, status));

    [HttpPost("requests")]
    public async Task<ActionResult<HrLeaveRequestDto>> CreateRequest(int schoolId, [FromBody] HrLeaveRequestDto dto)
    { var r = await _service.CreateRequestAsync(dto); await _pushService.SendToPersonTypesAsync("HR Leave Request", "A new leave request has been submitted", new[] { "Staff" }, schoolId); return Ok(r); }


    // Leave Types
    [HttpGet("types")]
    public async Task<ActionResult<List<HrLeaveTypeDto>>> GetLeaveTypes()
        => Ok(await _service.GetLeaveTypesAsync());

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
    public async Task<ActionResult<List<HrHolidayDto>>> GetHolidays(int schoolId, [FromQuery] int? year)
        => Ok(await _service.GetHolidaysBySchoolIdAsync(schoolId, year));
                                                  
}
