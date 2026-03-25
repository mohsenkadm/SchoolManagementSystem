using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة ورديات العمل
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/hr/work-shifts")]
[Authorize]
public class HrWorkShiftsApiController : ControllerBase
{
    private readonly IHrWorkShiftService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrWorkShiftsApiController(IHrWorkShiftService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    [HttpGet]
    public async Task<ActionResult<List<HrWorkShiftDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));
            
}
