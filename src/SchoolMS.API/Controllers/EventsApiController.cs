using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الأحداث المدرسية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/events")]
[Authorize]
public class EventsApiController : ControllerBase
{
    private readonly ISchoolEventService _service;
    public EventsApiController(ISchoolEventService service) => _service = service;

    // جلب جميع أحداث المدرسة
    [HttpGet]
    public async Task<ActionResult<List<SchoolEventDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId));
                                        
}
