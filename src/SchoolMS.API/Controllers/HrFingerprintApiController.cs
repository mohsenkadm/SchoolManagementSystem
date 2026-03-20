using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة أجهزة البصمة وسجلاتها
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/fingerprint")]
[Authorize]
public class HrFingerprintApiController : ControllerBase
{
    private readonly IHrFingerprintService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrFingerprintApiController(IHrFingerprintService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpPost("scan")]
    public async Task<ActionResult<HrFingerprintRecordDto>> RecordScan(int schoolId, [FromBody] HrFingerprintScanDto dto)
        => Ok(await _service.RecordScanAsync(dto));

    [HttpGet("records")]
    public async Task<ActionResult<List<HrFingerprintRecordDto>>> GetRecords(
        int schoolId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] int? employeeId)
        => Ok(await _service.GetRecordsAsync(fromDate, toDate, employeeId));

    [HttpPost("manual-entry")]
    public async Task<ActionResult<HrFingerprintRecordDto>> CreateManualEntry(int schoolId, [FromBody] HrFingerprintRecordDto dto)
        => Ok(await _service.CreateManualEntryAsync(dto));

    [HttpGet("today/{employeeId}")]
    public async Task<ActionResult<List<HrFingerprintRecordDto>>> GetTodayRecords(int schoolId, int employeeId)
        => Ok(await _service.GetTodayRecordsAsync(employeeId));

    [HttpGet("devices")]
    public async Task<ActionResult<List<HrFingerprintDeviceDto>>> GetDevices(int schoolId)
        => Ok(await _service.GetDevicesAsync());

    [HttpPost("devices")]
    public async Task<ActionResult<HrFingerprintDeviceDto>> CreateDevice(int schoolId, [FromBody] HrFingerprintDeviceDto dto)
    { var r = await _service.CreateDeviceAsync(dto); await _pushService.SendToPersonTypesAsync("New Fingerprint Device", $"{dto.DeviceName} has been registered", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut("devices")]
    public async Task<ActionResult<HrFingerprintDeviceDto>> UpdateDevice(int schoolId, [FromBody] HrFingerprintDeviceDto dto)
        => Ok(await _service.UpdateDeviceAsync(dto));

    [HttpDelete("devices/{id}")]
    public async Task<IActionResult> DeleteDevice(int schoolId, int id) { await _service.DeleteDeviceAsync(id); return Ok(); }
}
