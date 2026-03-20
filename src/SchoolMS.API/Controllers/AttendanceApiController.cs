using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الحضور والانصراف
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/attendance")]
[Authorize]
public class AttendanceApiController : ControllerBase
{
    private readonly IAttendanceService _service;
    public AttendanceApiController(IAttendanceService service) => _service = service;

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());

    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // حضور المعلم - يتم جلب معرف المعلم من التوكن
    [HttpGet("teacher")]
    public async Task<ActionResult<List<AttendanceDto>>> GetTeacherAttendance(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var personId = GetPersonIdFromToken();
        return Ok(await _service.GetByPersonAsync(personId, PersonType.Teacher, schoolId));
    }

    // حضور الموظف - يتم جلب معرف الموظف من التوكن
    [HttpGet("staff")]
    public async Task<ActionResult<List<AttendanceDto>>> GetStaffAttendance(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Staff") return Forbid();
        var personId = GetPersonIdFromToken();
        return Ok(await _service.GetByPersonAsync(personId, PersonType.Staff, schoolId));
    }

    // حضور الطالب - يتم جلب معرف الطالب من التوكن
    [HttpGet("student")]
    public async Task<ActionResult<List<AttendanceDto>>> GetStudentAttendance(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var personId = GetPersonIdFromToken();
        return Ok(await _service.GetByPersonAsync(personId, PersonType.Student, schoolId));
    }

    // حضور أبناء ولي الأمر - يتم جلب معرف ولي الأمر من التوكن
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<AttendanceDto>>> GetParentChildrenAttendance(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        return Ok(await _service.GetByParentChildrenAsync(parentId, schoolId));
    }
}
