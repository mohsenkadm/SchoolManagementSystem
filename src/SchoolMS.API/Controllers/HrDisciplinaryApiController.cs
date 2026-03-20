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

    [HttpGet]
    public async Task<ActionResult<List<HrDisciplinaryActionDto>>> GetAll([FromQuery] int? employeeId)
        => Ok(await _service.GetAllAsync(employeeId));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrDisciplinaryActionDto>> Get(int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrDisciplinaryActionDto>> Create(int schoolId, [FromBody] HrDisciplinaryActionDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("Disciplinary Action", "A new disciplinary action has been recorded", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut]
    public async Task<ActionResult<HrDisciplinaryActionDto>> Update([FromBody] HrDisciplinaryActionDto dto)
        => Ok(await _service.UpdateAsync(dto));

    // Violation Types
    [HttpGet("violation-types")]
    public async Task<ActionResult<List<HrViolationTypeDto>>> GetViolationTypes()
        => Ok(await _service.GetViolationTypesAsync());

    [HttpPost("violation-types")]
    public async Task<ActionResult<HrViolationTypeDto>> CreateViolationType([FromBody] HrViolationTypeDto dto)
        => Ok(await _service.CreateViolationTypeAsync(dto));

    [HttpPut("violation-types")]
    public async Task<ActionResult<HrViolationTypeDto>> UpdateViolationType([FromBody] HrViolationTypeDto dto)
        => Ok(await _service.UpdateViolationTypeAsync(dto));

    [HttpDelete("violation-types/{id}")]
    public async Task<IActionResult> DeleteViolationType(int id) { await _service.DeleteViolationTypeAsync(id); return Ok(); }
}
