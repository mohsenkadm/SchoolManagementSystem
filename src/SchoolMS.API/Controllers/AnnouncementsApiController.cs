using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الإعلانات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/announcements")]
[Authorize]
public class AnnouncementsApiController : ControllerBase
{
    private readonly IAnnouncementService _service;
    public AnnouncementsApiController(IAnnouncementService service) => _service = service;

    // جلب جميع إعلانات المدرسة
    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetAllAsync(schoolId, branchId));

}
