using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة موظفي الموارد البشرية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/employees")]
[Authorize]
public class HrEmployeesApiController : ControllerBase
{
    private readonly IHrEmployeeService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrEmployeesApiController(IHrEmployeeService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrEmployeeListDto>>> GetAll(int schoolId) => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<HrEmployeeDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrEmployeeDto>> Create(int schoolId, [FromBody] HrEmployeeDto dto)
    { var result = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Employee Added", $"{dto.FullName} has been added", new[] { "Staff" }, schoolId); return Ok(result); }

    [HttpPut]
    public async Task<ActionResult<HrEmployeeDto>> Update(int schoolId, [FromBody] HrEmployeeDto dto)
    { var result = await _service.UpdateAsync(dto); await _pushService.SendToPersonTypesAsync("Employee Updated", $"{dto.FullName} has been updated", new[] { "Staff" }, schoolId); return Ok(result); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet("department/{departmentId}")]
    public async Task<ActionResult<List<HrEmployeeListDto>>> GetByDepartment(int schoolId, int departmentId)
        => Ok(await _service.GetByDepartmentAsync(departmentId));

    [HttpGet("branch/{branchId}")]
    public async Task<ActionResult<List<HrEmployeeListDto>>> GetByBranch(int schoolId, int branchId)
        => Ok(await _service.GetByBranchAsync(branchId));

    [HttpGet("generate-number")]
    public async Task<ActionResult<string>> GenerateNumber(int schoolId)
        => Ok(await _service.GenerateEmployeeNumberAsync());
}
