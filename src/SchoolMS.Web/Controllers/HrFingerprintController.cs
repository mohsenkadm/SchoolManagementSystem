using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrFingerprintController : Controller
{
    private readonly IHrFingerprintService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;
    public HrFingerprintController(IHrFingerprintService service, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService) { _service = service; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrFingerprint", "View")]
    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int? employeeId)
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
        return View(await _service.GetRecordsAsync(fromDate ?? DateTime.UtcNow.Date, toDate ?? DateTime.UtcNow.Date, employeeId));
    }

    [HttpPost, HasPermission("HrFingerprint", "Add")]
    public async Task<IActionResult> Scan([FromBody] HrFingerprintScanDto dto)
        => Json(await _service.RecordScanAsync(dto));

    [HttpPost, HasPermission("HrFingerprint", "Add")]
    public async Task<IActionResult> ManualEntry([FromBody] HrFingerprintRecordDto dto)
        => Json(await _service.CreateManualEntryAsync(dto));

    [HasPermission("HrFingerprint", "View")]
    public async Task<IActionResult> TodayRecords(int employeeId)
        => Json(await _service.GetTodayRecordsAsync(employeeId));

    // Devices
    [HasPermission("HrFingerprint", "View")]
    public async Task<IActionResult> Devices() => View(await _service.GetDevicesAsync());

    [HasPermission("HrFingerprint", "Add")]
    public IActionResult CreateDevice() => View();

    [HttpPost, HasPermission("HrFingerprint", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDevice(HrFingerprintDeviceDto dto) { await _service.CreateDeviceAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("New Fingerprint Device", $"{dto.DeviceName} has been registered", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Devices)); }

    [HttpPost, HasPermission("HrFingerprint", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDevice(HrFingerprintDeviceDto dto) { await _service.UpdateDeviceAsync(dto); return RedirectToAction(nameof(Devices)); }

    [HttpDelete("device/{id}"), HasPermission("HrFingerprint", "Delete")]
    public async Task<IActionResult> DeleteDevice(int id) { await _service.DeleteDeviceAsync(id); return Ok(); }
}
