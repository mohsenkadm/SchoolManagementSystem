using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة رواتب الموظفين والبدلات والخصومات والرواتب الشهرية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/salary")]
[Authorize]
public class HrSalaryApiController : ControllerBase
{
    private readonly IHrSalaryService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrSalaryApiController(IHrSalaryService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;

    // Salary Setup — my salary
    [HttpGet("my-salary")]
    public async Task<ActionResult<HrSalaryDetailDto>> GetMySalary()
    {
        var empId = GetEmployeeIdFromToken();
        if (!empId.HasValue) return Unauthorized();
        var item = await _service.GetCurrentSalaryAsync(empId.Value);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("my-salary/history")]
    public async Task<ActionResult<List<HrSalaryDetailDto>>> GetMySalaryHistory()
    {
        var empId = GetEmployeeIdFromToken();
        if (!empId.HasValue) return Unauthorized();
        return Ok(await _service.GetSalaryHistoryAsync(empId.Value));
    }                                      
    // Allowance Types
    [HttpGet("allowance-types")]
    public async Task<ActionResult<List<HrAllowanceTypeDto>>> GetAllowanceTypes()
        => Ok(await _service.GetAllowanceTypesAsync());

    // Deduction Types
    [HttpGet("deduction-types")]
    public async Task<ActionResult<List<HrDeductionTypeDto>>> GetDeductionTypes()
        => Ok(await _service.GetDeductionTypesAsync());
                      
    // Advances — returns only the logged-in employee's data
    [HttpGet("advances")]
    public async Task<ActionResult<List<HrSalaryAdvanceDto>>> GetAdvances(int schoolId, [FromQuery] AdvanceStatus? status)
        => Ok(await _service.GetAdvancesBySchoolIdAsync(schoolId, status, GetEmployeeIdFromToken()));

    // Loans
    [HttpGet("loans")]
    public async Task<ActionResult<List<HrEmployeeLoanDto>>> GetLoans(int schoolId)
        => Ok(await _service.GetLoansBySchoolIdAsync(schoolId, GetEmployeeIdFromToken()));

    // Bonuses
    [HttpGet("bonuses")]
    public async Task<ActionResult<List<HrBonusDto>>> GetBonuses(int schoolId, [FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetBonusesBySchoolIdAsync(schoolId, month, year, GetEmployeeIdFromToken()));

    // Penalties
    [HttpGet("penalties")]
    public async Task<ActionResult<List<HrPenaltyDto>>> GetPenalties(int schoolId, [FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetPenaltiesBySchoolIdAsync(schoolId, month, year, GetEmployeeIdFromToken()));

}
