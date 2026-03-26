using System.Security.Claims;
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

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;


    // Returns logged-in employee's contracts
    [HttpGet("my-contracts")]
    public async Task<ActionResult<List<HrEmployeeContractDto>>> GetMyContracts()
    {
        var empId = GetEmployeeIdFromToken();
        if (!empId.HasValue) return Unauthorized();
        return Ok(await _service.GetByEmployeeAsync(empId.Value));
    }


}
