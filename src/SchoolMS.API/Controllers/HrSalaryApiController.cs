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

    [HttpPost("setup")]
    public async Task<ActionResult<HrSalaryDetailDto>> CreateSetup(int schoolId, [FromBody] HrSalaryDetailDto dto)
    { var r = await _service.CreateSalarySetupAsync(dto); await _pushService.SendToPersonTypesAsync("Salary Setup Created", "A new salary setup has been configured", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut("setup")]
    public async Task<ActionResult<HrSalaryDetailDto>> UpdateSetup([FromBody] HrSalaryDetailDto dto)
        => Ok(await _service.UpdateSalarySetupAsync(dto));

    // Allowance Types
    [HttpGet("allowance-types")]
    public async Task<ActionResult<List<HrAllowanceTypeDto>>> GetAllowanceTypes()
        => Ok(await _service.GetAllowanceTypesAsync());

    [HttpPost("allowance-types")]
    public async Task<ActionResult<HrAllowanceTypeDto>> CreateAllowanceType([FromBody] HrAllowanceTypeDto dto)
        => Ok(await _service.CreateAllowanceTypeAsync(dto));

    [HttpPut("allowance-types")]
    public async Task<ActionResult<HrAllowanceTypeDto>> UpdateAllowanceType([FromBody] HrAllowanceTypeDto dto)
        => Ok(await _service.UpdateAllowanceTypeAsync(dto));

    [HttpDelete("allowance-types/{id}")]
    public async Task<IActionResult> DeleteAllowanceType(int id) { await _service.DeleteAllowanceTypeAsync(id); return Ok(); }

    // Deduction Types
    [HttpGet("deduction-types")]
    public async Task<ActionResult<List<HrDeductionTypeDto>>> GetDeductionTypes()
        => Ok(await _service.GetDeductionTypesAsync());

    [HttpPost("deduction-types")]
    public async Task<ActionResult<HrDeductionTypeDto>> CreateDeductionType([FromBody] HrDeductionTypeDto dto)
        => Ok(await _service.CreateDeductionTypeAsync(dto));

    [HttpPut("deduction-types")]
    public async Task<ActionResult<HrDeductionTypeDto>> UpdateDeductionType([FromBody] HrDeductionTypeDto dto)
        => Ok(await _service.UpdateDeductionTypeAsync(dto));

    [HttpDelete("deduction-types/{id}")]
    public async Task<IActionResult> DeleteDeductionType(int id) { await _service.DeleteDeductionTypeAsync(id); return Ok(); }

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

    [HttpPost("payroll/{id}/approve")]
    public async Task<IActionResult> ApprovePayroll(int id, [FromQuery] string approvedBy)
    {
        await _service.ApprovePayrollAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("payroll/{id}/mark-paid")]
    public async Task<IActionResult> MarkPayrollPaid(int id)
    {
        await _service.MarkPayrollPaidAsync(id);
        return Ok();
    }

    // Advances
    [HttpGet("advances")]
    public async Task<ActionResult<List<HrSalaryAdvanceDto>>> GetAdvances([FromQuery] AdvanceStatus? status)
        => Ok(await _service.GetAdvancesAsync(status));

    [HttpPost("advances")]
    public async Task<ActionResult<HrSalaryAdvanceDto>> CreateAdvance([FromBody] HrSalaryAdvanceDto dto)
        => Ok(await _service.CreateAdvanceAsync(dto));

    [HttpPost("advances/{id}/approve")]
    public async Task<IActionResult> ApproveAdvance(int id, [FromQuery] string approvedBy,
        [FromQuery] decimal approvedAmount, [FromQuery] int deductionMonths)
    {
        await _service.ApproveAdvanceAsync(id, approvedBy, approvedAmount, deductionMonths);
        return Ok();
    }

    [HttpPost("advances/{id}/reject")]
    public async Task<IActionResult> RejectAdvance(int id, [FromQuery] string rejectedBy, [FromQuery] string reason)
    {
        await _service.RejectAdvanceAsync(id, rejectedBy, reason);
        return Ok();
    }

    // Loans
    [HttpGet("loans")]
    public async Task<ActionResult<List<HrEmployeeLoanDto>>> GetLoans([FromQuery] int? employeeId)
        => Ok(await _service.GetLoansAsync(employeeId));

    [HttpPost("loans")]
    public async Task<ActionResult<HrEmployeeLoanDto>> CreateLoan([FromBody] HrEmployeeLoanDto dto)
        => Ok(await _service.CreateLoanAsync(dto));

    // Bonuses
    [HttpGet("bonuses")]
    public async Task<ActionResult<List<HrBonusDto>>> GetBonuses([FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetBonusesAsync(month, year));

    [HttpPost("bonuses")]
    public async Task<ActionResult<HrBonusDto>> CreateBonus([FromBody] HrBonusDto dto)
        => Ok(await _service.CreateBonusAsync(dto));

    [HttpPost("bonuses/{id}/approve")]
    public async Task<IActionResult> ApproveBonus(int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveBonusAsync(id, approvedBy);
        return Ok();
    }

    // Penalties
    [HttpGet("penalties")]
    public async Task<ActionResult<List<HrPenaltyDto>>> GetPenalties([FromQuery] int? month, [FromQuery] int? year)
        => Ok(await _service.GetPenaltiesAsync(month, year));

    [HttpPost("penalties")]
    public async Task<ActionResult<HrPenaltyDto>> CreatePenalty([FromBody] HrPenaltyDto dto)
        => Ok(await _service.CreatePenaltyAsync(dto));

    [HttpPost("penalties/{id}/approve")]
    public async Task<IActionResult> ApprovePenalty(int id, [FromQuery] string approvedBy)
    {
        await _service.ApprovePenaltyAsync(id, approvedBy);
        return Ok();
    }
}
