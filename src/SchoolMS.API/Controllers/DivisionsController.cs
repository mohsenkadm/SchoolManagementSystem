using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الشعب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/divisions")]
[Authorize]
public class DivisionsController : ControllerBase
{
    private readonly IDivisionService _service;
    public DivisionsController(IDivisionService service) => _service = service;

    // جلب جميع الشعب للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<DivisionDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));

}
