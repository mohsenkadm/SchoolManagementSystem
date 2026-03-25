using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المسميات الوظيفية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/job-titles")]
[Authorize]
public class HrJobTitlesApiController : ControllerBase
{
    private readonly IHrJobTitleService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrJobTitlesApiController(IHrJobTitleService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrJobTitleDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));
                                                
}
