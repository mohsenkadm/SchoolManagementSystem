using System.Security.Claims;
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

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;

    [HttpGet]
    public async Task<ActionResult<List<HrOvertimeRequestDto>>> GetAll(int schoolId, [FromQuery] OvertimeStatus? status)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, status, GetEmployeeIdFromToken()));

}
