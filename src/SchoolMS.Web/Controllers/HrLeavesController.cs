using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrLeavesController : Controller
{
    private readonly IHrLeaveService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public HrLeavesController(IHrLeaveService service, IHrEmployeeService empService, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    // Leave Requests
    [HasPermission("HrLeaves", "View")]
    public async Task<IActionResult> Index(HrLeaveStatus? status)
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
        return View(await _service.GetAllRequestsAsync(status));
    }

    [HasPermission("HrLeaves", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = await _empService.GetAllAsync();
        ViewBag.LeaveTypes = await _service.GetLeaveTypesAsync();
        return View();
    }

    [HttpPost, HasPermission("HrLeaves", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrLeaveRequestDto dto) { await _service.CreateRequestAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("HR Leave Request", "A new leave request has been submitted", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrLeaves", "Edit")]
    public async Task<IActionResult> ApproveManager(int id) { await _service.ApproveByManagerAsync(id, User.Identity?.Name ?? ""); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Leave Approved by Manager", "A leave request has been approved by manager", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrLeaves", "Edit")]
    public async Task<IActionResult> ApproveHr(int id) { await _service.ApproveByHrAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrLeaves", "Edit")]
    public async Task<IActionResult> Reject(int id, string reason) { await _service.RejectAsync(id, User.Identity?.Name ?? "", reason); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Leave Rejected", "A leave request has been rejected", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrLeaves", "Edit")]
    public async Task<IActionResult> Cancel(int id) { await _service.CancelAsync(id); return RedirectToAction(nameof(Index)); }

    // Leave Types
    [HasPermission("HrLeaveTypes", "View")]
    public async Task<IActionResult> Types() => View(await _service.GetLeaveTypesAsync());

    [HttpPost, HasPermission("HrLeaveTypes", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateType(HrLeaveTypeDto dto) { await _service.CreateLeaveTypeAsync(dto); return RedirectToAction(nameof(Types)); }

    [HttpPost, HasPermission("HrLeaveTypes", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateType(HrLeaveTypeDto dto) { await _service.UpdateLeaveTypeAsync(dto); return RedirectToAction(nameof(Types)); }

    [HttpDelete("type/{id}"), HasPermission("HrLeaveTypes", "Delete")]
    public async Task<IActionResult> DeleteType(int id) { await _service.DeleteLeaveTypeAsync(id); return Ok(); }

    // Leave Balances
    [HasPermission("HrLeaves", "View")]
    public async Task<IActionResult> Balances(int employeeId) => View(await _service.GetBalancesAsync(employeeId));

    [HasPermission("HrLeaves", "View")]
    public async Task<IActionResult> AllBalances(int year) => View(await _service.GetAllBalancesAsync(year));

    [HttpPost, HasPermission("HrLeaves", "Add")]
    public async Task<IActionResult> InitializeBalances(int employeeId, int year) { await _service.InitializeBalancesAsync(employeeId, year); return RedirectToAction(nameof(Balances), new { employeeId }); }

    // Holidays
    [HasPermission("HrHolidays", "View")]
    public async Task<IActionResult> Holidays(int? year) => View(await _service.GetHolidaysAsync(year));

    [HttpPost, HasPermission("HrHolidays", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHoliday(HrHolidayDto dto) { await _service.CreateHolidayAsync(dto); return RedirectToAction(nameof(Holidays)); }

    [HttpPost, HasPermission("HrHolidays", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateHoliday(HrHolidayDto dto) { await _service.UpdateHolidayAsync(dto); return RedirectToAction(nameof(Holidays)); }

    [HttpDelete("holiday/{id}"), HasPermission("HrHolidays", "Delete")]
    public async Task<IActionResult> DeleteHoliday(int id) { await _service.DeleteHolidayAsync(id); return Ok(); }
}
