using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة أنواع الامتحانات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/exam-types")]
[Authorize]
public class ExamTypesApiController : ControllerBase
{
    private readonly IExamTypeService _service;
    public ExamTypesApiController(IExamTypeService service) => _service = service;

    // جلب جميع أنواع الامتحانات للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<ExamTypeDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

}
