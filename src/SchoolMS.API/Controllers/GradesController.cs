using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المراحل الدراسية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/grades")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly IGradeService _service;
    public GradesController(IGradeService service) => _service = service;

    // جلب جميع المراحل الدراسية للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<GradeDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId));
                          
}
