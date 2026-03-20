using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة السجلات الصحية للطلاب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/health-records")]
[Authorize]
public class HealthRecordsApiController : ControllerBase
{
    private readonly IHealthRecordService _service;
    public HealthRecordsApiController(IHealthRecordService service) => _service = service;

    // جلب جميع السجلات الصحية للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<HealthRecordDto>>> GetAll(int schoolId, [FromQuery] int? studentId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, studentId));

    // جلب السجلات الصحية لأبناء ولي الأمر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<HealthRecordDto>>> GetParentChildrenRecords(int schoolId)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Parent") return Forbid();
        var parentId = int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());
        return Ok(await _service.GetByParentChildrenAsync(parentId, schoolId));
    }
}
