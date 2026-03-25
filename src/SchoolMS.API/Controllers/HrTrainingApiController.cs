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
    public async Task<ActionResult<List<HrTrainingProgramDto>>> GetPrograms(int schoolId)
        => Ok(await _service.GetProgramsBySchoolIdAsync(schoolId));

    // Records
    [HttpGet("records")]
    public async Task<ActionResult<List<HrTrainingRecordDto>>> GetRecords(int schoolId,
        [FromQuery] int? programId, [FromQuery] int? employeeId)
        => Ok(await _service.GetRecordsBySchoolIdAsync(schoolId, programId, employeeId));
                                    

    // Certificates
    [HttpGet("certificates/{employeeId}")]
    public async Task<ActionResult<List<HrProfessionalCertificateDto>>> GetCertificates(int employeeId)
        => Ok(await _service.GetCertificatesAsync(employeeId));

  
}
