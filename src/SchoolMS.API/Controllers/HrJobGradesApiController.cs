using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الدرجات الوظيفية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/job-grades")]
[Authorize]
public class HrJobGradesApiController : ControllerBase
{
    private readonly IHrJobGradeService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrJobGradesApiController(IHrJobGradeService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrJobGradeDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));
                                           

    [HttpGet("{gradeId}/steps")]
    public async Task<ActionResult<List<HrJobGradeStepDto>>> GetSteps(int schoolId, int gradeId)
        => Ok(await _service.GetStepsAsync(gradeId));
                                                    
}
