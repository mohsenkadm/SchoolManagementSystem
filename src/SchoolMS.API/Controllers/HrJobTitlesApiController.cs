using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المسميات الوظيفية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/job-titles")]
[Authorize]
public class HrJobTitlesApiController : ControllerBase
{
    private readonly IHrJobTitleService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrJobTitlesApiController(IHrJobTitleService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrJobTitleDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrJobTitleDto>> GetById(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrJobTitleDto>> Create(int schoolId, [FromBody] HrJobTitleDto dto)
    {
        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Job Title",
            $"{dto.TitleName} has been created",
            new[] { "Staff" }, schoolId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<HrJobTitleDto>> Update(int schoolId, [FromBody] HrJobTitleDto dto)
        => Ok(await _service.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}
