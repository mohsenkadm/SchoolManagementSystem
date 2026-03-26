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
    public async Task<ActionResult<List<HrJobGradeDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrJobGradeDto>> GetById(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("{gradeId}/steps")]
    public async Task<ActionResult<List<HrJobGradeStepDto>>> GetSteps(int schoolId, int gradeId)
        => Ok(await _service.GetStepsAsync(gradeId));

    [HttpPost]
    public async Task<ActionResult<HrJobGradeDto>> Create(int schoolId, [FromBody] HrJobGradeDto dto)
    {
        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Job Grade",
            $"{dto.GradeName} has been created",
            new[] { "Staff" }, schoolId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<HrJobGradeDto>> Update(int schoolId, [FromBody] HrJobGradeDto dto)
        => Ok(await _service.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("steps")]
    public async Task<ActionResult<HrJobGradeStepDto>> AddStep(int schoolId, [FromBody] HrJobGradeStepDto dto)
        => Ok(await _service.CreateStepAsync(dto));

    [HttpDelete("steps/{id}")]
    public async Task<IActionResult> DeleteStep(int schoolId, int id)
    {
        await _service.DeleteStepAsync(id);
        return Ok();
    }
}
