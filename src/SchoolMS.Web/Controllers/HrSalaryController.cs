using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrSalaryController : Controller
{
    private readonly IHrSalaryService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public HrSalaryController(IHrSalaryService service, IHrEmployeeService empService, IBranchService branchService,
        IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _branchService = branchService; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    // Salary Setup
    [HasPermission("HrSalary", "View")]
    public async Task<IActionResult> Setup(int employeeId) => View(await _service.GetCurrentSalaryAsync(employeeId));

    [HasPermission("HrSalary", "View")]
    public async Task<IActionResult> History(int employeeId) => Json(await _service.GetSalaryHistoryAsync(employeeId));

    [HttpPost, HasPermission("HrSalary", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSetup(HrSalaryDetailDto dto) { await _service.CreateSalarySetupAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Salary Setup Created", "A new salary setup has been configured", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction("Details", "HrEmployees", new { id = dto.EmployeeId }); }

    // Allowance Types
    [HasPermission("HrSalary", "View")]
    public async Task<IActionResult> AllowanceTypes() => View(await _service.GetAllowanceTypesAsync());

    [HttpPost, HasPermission("HrSalary", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAllowanceType(HrAllowanceTypeDto dto) { await _service.CreateAllowanceTypeAsync(dto); return RedirectToAction(nameof(AllowanceTypes)); }

    [HttpPost, HasPermission("HrSalary", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAllowanceType(HrAllowanceTypeDto dto) { await _service.UpdateAllowanceTypeAsync(dto); return RedirectToAction(nameof(AllowanceTypes)); }

    [HttpDelete("allowance-type/{id}"), HasPermission("HrSalary", "Delete")]
    public async Task<IActionResult> DeleteAllowanceType(int id) { await _service.DeleteAllowanceTypeAsync(id); return Ok(); }

    // Deduction Types
    [HasPermission("HrSalary", "View")]
    public async Task<IActionResult> DeductionTypes() => View(await _service.GetDeductionTypesAsync());

    [HttpPost, HasPermission("HrSalary", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDeductionType(HrDeductionTypeDto dto) { await _service.CreateDeductionTypeAsync(dto); return RedirectToAction(nameof(DeductionTypes)); }

    [HttpPost, HasPermission("HrSalary", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDeductionType(HrDeductionTypeDto dto) { await _service.UpdateDeductionTypeAsync(dto); return RedirectToAction(nameof(DeductionTypes)); }

    [HttpDelete("deduction-type/{id}"), HasPermission("HrSalary", "Delete")]
    public async Task<IActionResult> DeleteDeductionType(int id) { await _service.DeleteDeductionTypeAsync(id); return Ok(); }

    // Payroll
    [HasPermission("HrPayroll", "View")]
    public async Task<IActionResult> Payroll(int? year) => View(await _service.GetPayrollListAsync(year));

    [HasPermission("HrPayroll", "View")]
    public async Task<IActionResult> PayrollDetails(int month, int year, int branchId) => View(await _service.GetPayrollAsync(month, year, branchId));

    [HttpPost, HasPermission("HrPayroll", "Add")]
    public async Task<IActionResult> GeneratePayroll(int month, int year, int branchId)
    {
        await _service.GeneratePayrollAsync(month, year, branchId);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Payroll Generated",
                $"Payroll for {month}/{year} has been generated",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Payroll));
    }

    [HasPermission("HrPayroll", "Add")]
    public async Task<IActionResult> GeneratePayrollForm()
    {
        ViewBag.Branches = await _branchService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("HrPayroll", "Edit")]
    public async Task<IActionResult> ApprovePayroll(int payrollId)
    {
        await _service.ApprovePayrollAsync(payrollId, User.Identity?.Name ?? "");
        return RedirectToAction(nameof(Payroll));
    }

    [HttpPost, HasPermission("HrPayroll", "Edit")]
    public async Task<IActionResult> MarkPayrollPaid(int payrollId)
    {
        await _service.MarkPayrollPaidAsync(payrollId);
        return RedirectToAction(nameof(Payroll));
    }

    // Advances
    [HasPermission("HrAdvances", "View")]
    public async Task<IActionResult> Advances(AdvanceStatus? status) => View(await _service.GetAdvancesAsync(status));

    [HasPermission("HrAdvances", "Add")]
    public async Task<IActionResult> CreateAdvance() { ViewBag.Employees = await _empService.GetAllAsync(); return View(); }

    [HttpPost, HasPermission("HrAdvances", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateAdvance(HrSalaryAdvanceDto dto) { await _service.CreateAdvanceAsync(dto); return RedirectToAction(nameof(Advances)); }

    [HttpPost, HasPermission("HrAdvances", "Edit")]
    public async Task<IActionResult> ApproveAdvance(int id, decimal approvedAmount, int deductionMonths)
    {
        await _service.ApproveAdvanceAsync(id, User.Identity?.Name ?? "", approvedAmount, deductionMonths);
        return RedirectToAction(nameof(Advances));
    }

    [HttpPost, HasPermission("HrAdvances", "Edit")]
    public async Task<IActionResult> RejectAdvance(int id, string reason)
    {
        await _service.RejectAdvanceAsync(id, User.Identity?.Name ?? "", reason);
        return RedirectToAction(nameof(Advances));
    }

    // Loans
    [HasPermission("HrLoans", "View")]
    public async Task<IActionResult> Loans(int? employeeId) => View(await _service.GetLoansAsync(employeeId));

    [HttpPost, HasPermission("HrLoans", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLoan(HrEmployeeLoanDto dto) { await _service.CreateLoanAsync(dto); return RedirectToAction(nameof(Loans)); }

    // Bonuses
    [HasPermission("HrBonuses", "View")]
    public async Task<IActionResult> Bonuses(int? month, int? year) => View(await _service.GetBonusesAsync(month, year));

    [HttpPost, HasPermission("HrBonuses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBonus(HrBonusDto dto) { await _service.CreateBonusAsync(dto); return RedirectToAction(nameof(Bonuses)); }

    [HttpPost, HasPermission("HrBonuses", "Edit")]
    public async Task<IActionResult> ApproveBonus(int id) { await _service.ApproveBonusAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Bonuses)); }

    // Penalties
    [HasPermission("HrPenalties", "View")]
    public async Task<IActionResult> Penalties(int? month, int? year) => View(await _service.GetPenaltiesAsync(month, year));

    [HttpPost, HasPermission("HrPenalties", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePenalty(HrPenaltyDto dto) { await _service.CreatePenaltyAsync(dto); return RedirectToAction(nameof(Penalties)); }

    [HttpPost, HasPermission("HrPenalties", "Edit")]
    public async Task<IActionResult> ApprovePenalty(int id) { await _service.ApprovePenaltyAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Penalties)); }
}
