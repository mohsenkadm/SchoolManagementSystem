using System.Security.Claims;
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

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;

    [HttpGet("daily")]
    public async Task<ActionResult<List<HrDailyAttendanceDto>>> GetDaily(
        int schoolId, [FromQuery] DateTime date, [FromQuery] int? departmentId, [FromQuery] int? branchId)
        => Ok(await _service.GetDailyAttendanceAsync(date, departmentId, branchId, GetEmployeeIdFromToken()));


    [HttpGet("monthly")]
    public async Task<ActionResult<List<HrDailyAttendanceDto>>> GetMonthlyByEmployee(
        int schoolId,  [FromQuery] int month, [FromQuery] int year)
        => Ok(await _service.GetMonthlyAttendanceAsync(GetEmployeeIdFromToken()??0, month, year));

}
