using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الدرجات الوظيفية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/job-grades")]
[Authorize]
public class HrJobGradesApiController : ControllerBase
{
    private readonly IHrJobGradeService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrJobGradesApiController(IHrJobGradeService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrJobGradeDto>>> GetAll(int schoolId) => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HrJobGradeDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrJobGradeDto>> Create(int schoolId, [FromBody] HrJobGradeDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Job Grade", $"{dto.GradeName} has been created", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut]
    public async Task<ActionResult<HrJobGradeDto>> Update(int schoolId, [FromBody] HrJobGradeDto dto)
    { var r = await _service.UpdateAsync(dto); await _pushService.SendToPersonTypesAsync("Job Grade Updated", $"{dto.GradeName} has been updated", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet("{gradeId}/steps")]
    public async Task<ActionResult<List<HrJobGradeStepDto>>> GetSteps(int schoolId, int gradeId)
        => Ok(await _service.GetStepsAsync(gradeId));

    [HttpPost("steps")]
    public async Task<ActionResult<HrJobGradeStepDto>> CreateStep(int schoolId, [FromBody] HrJobGradeStepDto dto)
        => Ok(await _service.CreateStepAsync(dto));

    [HttpDelete("steps/{stepId}")]
    public async Task<IActionResult> DeleteStep(int schoolId, int stepId) { await _service.DeleteStepAsync(stepId); return Ok(); }
}
