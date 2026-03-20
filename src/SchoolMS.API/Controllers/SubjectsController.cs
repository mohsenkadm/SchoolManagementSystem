using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المواد الدراسية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/subjects")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;
    public SubjectsController(ISubjectService service) => _service = service;

    // جلب جميع المواد الدراسية للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<SubjectDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));
}
