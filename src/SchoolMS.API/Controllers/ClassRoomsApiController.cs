using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الفصول الدراسية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/classrooms")]
[Authorize]
public class ClassRoomsApiController : ControllerBase
{
    private readonly IClassRoomService _service;
    public ClassRoomsApiController(IClassRoomService service) => _service = service;

    // جلب جميع الفصول الدراسية للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<ClassRoomDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId));

  
}
