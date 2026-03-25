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
    public async Task<ActionResult<List<HrEmployeeContractDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<HrEmployeeContractDto>>> GetByEmployee(int schoolId, int employeeId)
        => Ok(await _service.GetByEmployeeAsync(employeeId));
                         
}
