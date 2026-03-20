using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrAttendanceController : Controller
{
    private readonly IHrAttendanceService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrAttendanceController(IHrAttendanceService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrAttendance", "View")]
    public async Task<IActionResult> Index(DateTime? date, int? departmentId, int? branchId)
        => View(await _service.GetDailyAttendanceAsync(date ?? DateTime.UtcNow.Date, departmentId, branchId));

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
