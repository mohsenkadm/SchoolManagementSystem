using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الأقسام
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/departments")]
[Authorize]
public class HrDepartmentsApiController : ControllerBase
{
    private readonly IHrDepartmentService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrDepartmentsApiController(IHrDepartmentService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrDepartmentDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrDepartmentDto>> GetById(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrDepartmentDto>> Create(int schoolId, [FromBody] HrDepartmentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Department",
            $"{dto.DepartmentName} department has been created",
            new[] { "Staff" }, schoolId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<HrDepartmentDto>> Update(int schoolId, [FromBody] HrDepartmentDto dto)
        => Ok(await _service.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}
