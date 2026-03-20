using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الحضور اليومي للموظفين
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/attendance")]
[Authorize]
public class HrAttendanceApiController : ControllerBase
{
    private readonly IHrAttendanceService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrAttendanceApiController(IHrAttendanceService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet("daily")]
    public async Task<ActionResult<List<HrDailyAttendanceDto>>> GetDaily(
        int schoolId, [FromQuery] DateTime date, [FromQuery] int? departmentId, [FromQuery] int? branchId)
        => Ok(await _service.GetDailyAttendanceAsync(date, departmentId, branchId));

    [HttpGet("monthly/{employeeId}")]
    public async Task<ActionResult<List<HrDailyAttendanceDto>>> GetMonthly(
        int schoolId, int employeeId, [FromQuery] int month, [FromQuery] int year)
        => Ok(await _service.GetMonthlyAttendanceAsync(employeeId, month, year));

    [HttpPost("process")]
    public async Task<IActionResult> ProcessDaily(int schoolId, [FromQuery] DateTime date)
    {
        await _service.ProcessDailyAttendanceAsync(date);
        await _pushService.SendToPersonTypesAsync("Attendance Processed", $"Daily attendance for {date:d} has been processed", new[] { "Staff" }, schoolId);
        return Ok();
    }
}
