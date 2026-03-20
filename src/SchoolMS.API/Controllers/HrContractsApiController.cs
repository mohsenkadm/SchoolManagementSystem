using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة عقود الموظفين
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/contracts")]
[Authorize]
public class HrContractsApiController : ControllerBase
{
    private readonly IHrContractService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrContractsApiController(IHrContractService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrEmployeeContractDto>>> GetAll(int schoolId) => Ok(await _service.GetAllAsync());

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<HrEmployeeContractDto>>> GetByEmployee(int schoolId, int employeeId)
        => Ok(await _service.GetByEmployeeAsync(employeeId));

    [HttpGet("{id}")]
    public async Task<ActionResult<HrEmployeeContractDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HrEmployeeContractDto>> Create(int schoolId, [FromBody] HrEmployeeContractDto dto)
    { var result = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Contract", "A new employee contract has been created", new[] { "Staff" }, schoolId); return Ok(result); }

    [HttpPut]
    public async Task<ActionResult<HrEmployeeContractDto>> Update(int schoolId, [FromBody] HrEmployeeContractDto dto)
    { var result = await _service.UpdateAsync(dto); await _pushService.SendToPersonTypesAsync("Contract Updated", "An employee contract has been updated", new[] { "Staff" }, schoolId); return Ok(result); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id) { await _service.DeleteAsync(id); return Ok(); }
}
