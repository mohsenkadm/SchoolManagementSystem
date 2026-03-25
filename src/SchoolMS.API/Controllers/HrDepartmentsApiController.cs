using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة أقسام الموارد البشرية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/departments")]
[Authorize]
public class HrDepartmentsApiController : ControllerBase
{
    private readonly IHrDepartmentService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrDepartmentsApiController(IHrDepartmentService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrDepartmentDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));
                                           
}
