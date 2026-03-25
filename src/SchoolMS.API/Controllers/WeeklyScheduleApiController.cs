using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الجدول الأسبوعي للحصص
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/weekly-schedule")]
[Authorize]
public class WeeklyScheduleApiController : ControllerBase
{
    private readonly IWeeklyScheduleService _service;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<TeacherAssignment> _assignmentRepo;

    public WeeklyScheduleApiController(
        IWeeklyScheduleService service,
        IRepository<Student> studentRepo,
        IRepository<TeacherAssignment> assignmentRepo)
    {
        _service = service;
        _studentRepo = studentRepo;
        _assignmentRepo = assignmentRepo;
    }

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());
    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // جلب الجدول الأسبوعي للمدرسة مع فلاتر
    [HttpGet]
    public async Task<ActionResult<List<WeeklyScheduleDto>>> GetAll(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] int? teacherId = null,
        [FromQuery] DayOfWeek? dayOfWeek = null, [FromQuery] int? classRoomId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, subjectId, teacherId, dayOfWeek, classRoomId));

    // جدول الطالب - حسب فصل الطالب مع فلاتر
    [HttpGet("student")]
    public async Task<ActionResult<List<WeeklyScheduleDto>>> GetStudentSchedule(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] DayOfWeek? dayOfWeek = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);
        if (student == null) return NotFound();
        return Ok(await _service.GetByClassRoomIdsAsync(new List<int> { student.ClassRoomId }, schoolId, subjectId, dayOfWeek));
    }

    // جدول المعلم - حسب الفصول المعينة للمعلم مع فلاتر
    [HttpGet("teacher")]
    public async Task<ActionResult<List<WeeklyScheduleDto>>> GetTeacherSchedule(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] DayOfWeek? dayOfWeek = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();
        var classRoomIds = await _assignmentRepo.Query()
            .Where(a => a.TeacherId == teacherId && a.SchoolId == schoolId)
            .Select(a => a.ClassRoomId).Distinct().ToListAsync();
        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId, subjectId, dayOfWeek));
    }

    // جدول أبناء ولي الأمر مع فلاتر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<WeeklyScheduleDto>>> GetParentChildrenSchedule(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] DayOfWeek? dayOfWeek = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        var classRoomIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.ClassRoomId).Distinct().ToListAsync();
        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId, subjectId, dayOfWeek));
    }
}
