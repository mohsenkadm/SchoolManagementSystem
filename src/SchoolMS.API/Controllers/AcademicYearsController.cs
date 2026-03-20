using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة السنوات الدراسية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/academic-years")]
[Authorize]
public class AcademicYearsController : ControllerBase
{
    private readonly IAcademicYearService _service;
    public AcademicYearsController(IAcademicYearService service) => _service = service;

    // جلب جميع السنوات الدراسية للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<AcademicYearDto>>> GetAll(int schoolId) => Ok(await _service.GetAllAsync(schoolId));
                    
    // جلب السنة الدراسية الحالية
    [HttpGet("current")]
    public async Task<ActionResult<AcademicYearDto>> GetCurrent(int schoolId)
    {
        var item = await _service.GetCurrentAsync(schoolId);
        return item == null ? NotFound() : Ok(item);
    }                                                                                                         
}
