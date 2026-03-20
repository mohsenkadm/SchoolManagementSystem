using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class InstallmentsController : Controller
{
    private readonly IInstallmentService _service;
    private readonly IStudentService _studentService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public InstallmentsController(IInstallmentService service, IStudentService studentService,
        IAcademicYearService yearService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _studentService = studentService; _yearService = yearService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    private async Task SetCommonViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
    }

    [HasPermission("Installments", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Installments & Fees";
        await SetCommonViewBags();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<FeeInstallmentDto>();
        return View(all);
    }

    [HasPermission("Installments", "View")]
    public async Task<IActionResult> PaymentsReport(string? status = null)
    {
        ViewData["Title"] = "Payments Report";
        await SetCommonViewBags();
        PaymentStatus? filter = status switch { "paid" => PaymentStatus.Paid, "pending" => PaymentStatus.Pending, "overdue" => PaymentStatus.Overdue, _ => null };
        ViewBag.CurrentFilter = status;
        var all = IsSuperAdmin
            ? await _service.GetAllPaymentsAsync(filter)
            : CurrentSchoolId.HasValue
                ? await _service.GetPaymentsBySchoolIdAsync(CurrentSchoolId.Value, filter)
                : new List<InstallmentPaymentDto>();
        return View(all);
    }

    [HasPermission("Installments", "View")]
    public async Task<IActionResult> StudentReport()
    {
        ViewData["Title"] = "Student Fee Summary";
        await SetCommonViewBags();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<FeeInstallmentDto>();
        return View(all);
    }

    [HasPermission("Installments", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Installment";
        await SetCommonViewBags();
        ViewBag.Students = await _studentService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        return View();
    }

    [HttpPost, HasPermission("Installments", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeeInstallmentDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("New Fee Installment",
            $"A fee of {dto.TotalAmount:N2} has been assigned to {dto.StudentName ?? "your child"}",
            dto.StudentId, "Parent", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Installments", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Installment";
        await SetCommonViewBags();
        ViewBag.Students = await _studentService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Installments", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FeeInstallmentDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToIndividualAsync("Fee Installment Updated",
            $"Fee installment for {dto.StudentName ?? "your child"} has been updated",
            dto.StudentId, "Parent", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Installments", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost, HasPermission("Installments", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordPayment(int paymentId)
    {
        await _service.RecordPaymentAsync(paymentId);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Payment Recorded",
                "A fee payment has been recorded",
                new[] { "Parent" }, CurrentSchoolId.Value);
        return Ok();
    }

    [HttpPost, HasPermission("Installments", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelPayment(int paymentId) { await _service.CancelPaymentAsync(paymentId); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Installments.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPaymentsExcel(string? status = null)
    {
        PaymentStatus? filter = status switch { "paid" => PaymentStatus.Paid, "pending" => PaymentStatus.Pending, "overdue" => PaymentStatus.Overdue, _ => null };
        var bytes = await _service.ExportPaymentsToExcelAsync(filter);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Payments_{status ?? "all"}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportStudentSummaryExcel()
    {
        var bytes = await _service.ExportStudentSummaryToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentFeeSummary.xlsx");
    }
}
