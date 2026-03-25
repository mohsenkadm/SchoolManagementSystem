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

    // Salary Setup
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<HrSalaryDetailDto>> GetCurrentSalary(int employeeId)
    {
        var item = await _service.GetCurrentSalaryAsync(employeeId);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("employee/{employeeId}/history")]
    public async Task<ActionResult<List<HrSalaryDetailDto>>> GetSalaryHistory(int employeeId)
        => Ok(await _service.GetSalaryHistoryAsync(employeeId));

    // Allowance Types
    [HttpGet("allowance-types")]
    public async Task<ActionResult<List<HrAllowanceTypeDto>>> GetAllowanceTypes()
        => Ok(await _service.GetAllowanceTypesAsync());

    // Deduction Types
    [HttpGet("deduction-types")]
    public async Task<ActionResult<List<HrDeductionTypeDto>>> GetDeductionTypes()
        => Ok(await _service.GetDeductionTypesAsync());

    // Payroll
    [HttpPost("payroll/generate")]
    public async Task<ActionResult<HrMonthlyPayrollDto>> GeneratePayroll(
        [FromQuery] int month, [FromQuery] int year, [FromQuery] int branchId)
        => Ok(await _service.GeneratePayrollAsync(month, year, branchId));

    [HttpGet("payroll")]
    public async Task<ActionResult<HrMonthlyPayrollDto>> GetPayroll(
        [FromQuery] int month, [FromQuery] int year, [FromQuery] int branchId)
    {
        var item = await _service.GetPayrollAsync(month, year, branchId);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpGet("payroll/list")]
    public async Task<ActionResult<List<HrMonthlyPayrollDto>>> GetPayrollList([FromQuery] int? year)
        => Ok(await _service.GetPayrollListAsync(year));


    // Advances
    [HttpGet("advances")]
    public async Task<ActionResult<List<HrSalaryAdvanceDto>>> GetAdvances(int schoolId, [FromQuery] AdvanceStatus? status)
        => Ok(await _service.GetAdvancesBySchoolIdAsync(schoolId, status));

    // Loans
    [HttpGet("loans")]
    public async Task<ActionResult<List<HrEmployeeLoanDto>>> GetLoans(int schoolId, [FromQuery] int? employeeId)
        => Ok(await _service.GetLoansBySchoolIdAsync(schoolId, employeeId));

    // Bonuses
    [HttpGet("bonuses")]
    public async Task<ActionResult<List<HrBonusDto>>> GetBonuses(int schoolId, [FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetBonusesBySchoolIdAsync(schoolId, month, year));


    // Penalties
    [HttpGet("penalties")]
    public async Task<ActionResult<List<HrPenaltyDto>>> GetPenalties(int schoolId, [FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetPenaltiesBySchoolIdAsync(schoolId, month, year));

}
