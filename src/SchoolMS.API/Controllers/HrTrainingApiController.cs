using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة برامج التدريب والشهادات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/training")]
[Authorize]
public class HrTrainingApiController : ControllerBase
{
    private readonly IHrTrainingService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrTrainingApiController(IHrTrainingService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    // Programs
    [HttpGet("programs")]
    public async Task<ActionResult<List<HrTrainingProgramDto>>> GetPrograms()
        => Ok(await _service.GetProgramsAsync());

    [HttpGet("programs/{id}")]
    public async Task<ActionResult<HrTrainingProgramDto>> GetProgram(int id)
    {
        var item = await _service.GetProgramByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost("programs")]
    public async Task<ActionResult<HrTrainingProgramDto>> CreateProgram(int schoolId, [FromBody] HrTrainingProgramDto dto)
    { var r = await _service.CreateProgramAsync(dto); await _pushService.SendToPersonTypesAsync("New Training Program", $"{dto.ProgramName} is now available", new[] { "Staff" }, schoolId); return Ok(r); }

    [HttpPut("programs")]
    public async Task<ActionResult<HrTrainingProgramDto>> UpdateProgram([FromBody] HrTrainingProgramDto dto)
        => Ok(await _service.UpdateProgramAsync(dto));

    [HttpDelete("programs/{id}")]
    public async Task<IActionResult> DeleteProgram(int id) { await _service.DeleteProgramAsync(id); return Ok(); }

    // Records
    [HttpGet("records")]
    public async Task<ActionResult<List<HrTrainingRecordDto>>> GetRecords(
        [FromQuery] int? programId, [FromQuery] int? employeeId)
        => Ok(await _service.GetRecordsAsync(programId, employeeId));

    [HttpPost("records")]
    public async Task<ActionResult<HrTrainingRecordDto>> EnrollEmployee([FromBody] HrTrainingRecordDto dto)
        => Ok(await _service.EnrollEmployeeAsync(dto));

    [HttpPut("records")]
    public async Task<ActionResult<HrTrainingRecordDto>> UpdateRecord([FromBody] HrTrainingRecordDto dto)
        => Ok(await _service.UpdateRecordAsync(dto));

    // Requests
    [HttpGet("requests")]
    public async Task<ActionResult<List<HrTrainingRequestDto>>> GetRequests([FromQuery] TrainingRequestStatus? status)
        => Ok(await _service.GetRequestsAsync(status));

    [HttpPost("requests")]
    public async Task<ActionResult<HrTrainingRequestDto>> CreateRequest([FromBody] HrTrainingRequestDto dto)
        => Ok(await _service.CreateRequestAsync(dto));

    [HttpPost("requests/{id}/approve")]
    public async Task<IActionResult> ApproveRequest(int id, [FromQuery] string approvedBy)
    {
        await _service.ApproveRequestAsync(id, approvedBy);
        return Ok();
    }

    [HttpPost("requests/{id}/reject")]
    public async Task<IActionResult> RejectRequest(int id, [FromQuery] string rejectedBy)
    {
        await _service.RejectRequestAsync(id, rejectedBy);
        return Ok();
    }

    // Certificates
    [HttpGet("certificates/{employeeId}")]
    public async Task<ActionResult<List<HrProfessionalCertificateDto>>> GetCertificates(int employeeId)
        => Ok(await _service.GetCertificatesAsync(employeeId));

    [HttpPost("certificates")]
    public async Task<ActionResult<HrProfessionalCertificateDto>> CreateCertificate([FromBody] HrProfessionalCertificateDto dto)
        => Ok(await _service.CreateCertificateAsync(dto));

    [HttpDelete("certificates/{id}")]
    public async Task<IActionResult> DeleteCertificate(int id) { await _service.DeleteCertificateAsync(id); return Ok(); }
}
