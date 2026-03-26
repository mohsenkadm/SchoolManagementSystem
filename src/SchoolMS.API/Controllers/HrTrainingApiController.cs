using System.Security.Claims;
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

    private int? GetEmployeeIdFromToken() => int.TryParse(User.FindFirst("PersonId")?.Value, out var id) ? id : null;
                               
    // Records — returns only the logged-in employee's training records
    [HttpGet("records")]
    public async Task<ActionResult<List<HrTrainingRecordDto>>> GetRecords(int schoolId, [FromQuery] int? programId)
        => Ok(await _service.GetRecordsBySchoolIdAsync(schoolId, programId, GetEmployeeIdFromToken()));

    // Certificates — returns the logged-in employee's certificates
    [HttpGet("certificates")]
    public async Task<ActionResult<List<HrProfessionalCertificateDto>>> GetMyCertificates()
    {
        var empId = GetEmployeeIdFromToken();
        if (!empId.HasValue) return Unauthorized();
        return Ok(await _service.GetCertificatesAsync(empId.Value));
    }
                                            

}
