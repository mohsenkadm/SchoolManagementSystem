using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة ورديات العمل
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/work-shifts")]
[Authorize]
public class HrWorkShiftsApiController : ControllerBase
{
    private readonly IHrWorkShiftService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrWorkShiftsApiController(IHrWorkShiftService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrWorkShiftDto>>> GetAll(int schoolId) => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HrWorkShiftDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrWorkShiftDto>> Create(int schoolId, [FromBody] HrWorkShiftDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Work Shift", $"{dto.ShiftName} has been created", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut]
    public async Task<ActionResult<HrWorkShiftDto>> Update(int schoolId, [FromBody] HrWorkShiftDto dto)
    { var r = await _service.UpdateAsync(dto); await _pushService.SendToPersonTypesAsync("Work Shift Updated", $"{dto.ShiftName} has been updated", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id) { await _service.DeleteAsync(id); return Ok(); }
}
