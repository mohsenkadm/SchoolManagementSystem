using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrAttendanceController : Controller
{
    private readonly IHrAttendanceService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;
    public HrAttendanceController(IHrAttendanceService service, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService) { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrAttendance", "View")]
    public async Task<IActionResult> Index(DateTime? date, int? departmentId, int? branchId)
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
        return View(await _service.GetDailyAttendanceAsync(date ?? DateTime.UtcNow.Date, departmentId, branchId));
    }

    [HasPermission("HrAttendance", "View")]
    public async Task<IActionResult> Monthly(int employeeId, int? month, int? year)
    {
        var now = DateTime.UtcNow;
        return Json(await _service.GetMonthlyAttendanceAsync(employeeId, month ?? now.Month, year ?? now.Year));
    }

    [HttpPost, HasPermission("HrAttendance", "Add")]
    public async Task<IActionResult> Process(DateTime date)
    {
        await _service.ProcessDailyAttendanceAsync(date);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Attendance Processed",
                $"Daily attendance for {date:d} has been processed",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index), new { date });
    }
}
