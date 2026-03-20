using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إعدادات الموارد البشرية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/settings")]
[Authorize]
public class HrSettingsApiController : ControllerBase
{
    private readonly IHrSettingsService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrSettingsApiController(IHrSettingsService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<HrSettingsDto>> GetSettings(int schoolId) => Ok(await _service.GetSettingsAsync());

    [HttpPut]
    public async Task<IActionResult> UpdateSettings(int schoolId, [FromBody] HrSettingsDto dto)
    {
        await _service.UpdateSettingsAsync(dto);
        await _pushService.SendToPersonTypesAsync("HR Settings Updated", "HR settings have been updated", new[] { "Staff" }, schoolId);
        return Ok();
    }
}
