using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة سلوك الطلاب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/behavior")]
[Authorize]
public class BehaviorApiController : ControllerBase
{
    private readonly IStudentBehaviorService _service;
    public BehaviorApiController(IStudentBehaviorService service) => _service = service;


    // جلب سجلات سلوك طالب معين
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<StudentBehaviorDto>>> GetByStudent(int schoolId, int studentId,
        [FromQuery] int? academicYearId = null)
        => Ok(await _service.GetByStudentIdAsync(studentId, academicYearId));

    // جلب سجلات سلوك أبناء ولي الأمر - يتم جلب معرف ولي الأمر من التوكن
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<StudentBehaviorDto>>> GetByParentChildren(int schoolId,
        [FromQuery] int? academicYearId = null)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Parent") return Forbid();
        var parentId = int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());
        return Ok(await _service.GetByParentChildrenAsync(parentId, schoolId, academicYearId));
    }
}
