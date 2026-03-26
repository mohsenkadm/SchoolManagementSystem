using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الإجراءات التأديبية والمخالفات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/disciplinary")]
[Authorize]
public class HrDisciplinaryApiController : ControllerBase
{
    private readonly IHrDisciplinaryService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrDisciplinaryApiController(IHrDisciplinaryService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;

    [HttpGet]
    public async Task<ActionResult<List<HrDisciplinaryActionDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, GetEmployeeIdFromToken()));

    // Violation Types
    [HttpGet("violation-types")]
    public async Task<ActionResult<List<HrViolationTypeDto>>> GetViolationTypes(int schoolId)
        => Ok(await _service.GetViolationTypesBySchoolIdAsync(schoolId));

}
