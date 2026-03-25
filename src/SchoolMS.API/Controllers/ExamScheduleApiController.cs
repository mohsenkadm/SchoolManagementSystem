using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة جداول الامتحانات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/exam-schedule")]
[Authorize]
public class ExamScheduleApiController : ControllerBase
{
    private readonly IExamScheduleService _service;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<TeacherAssignment> _assignmentRepo;

    public ExamScheduleApiController(
        IExamScheduleService service,
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

    // جلب جداول الامتحانات للمدرسة مع فلاتر اختيارية
    [HttpGet]
    public async Task<ActionResult<List<ExamScheduleDto>>> GetAll(int schoolId,
        [FromQuery] int? branchId = null, [FromQuery] int? examTypeId = null,
        [FromQuery] int? classRoomId = null, [FromQuery] int? subjectId = null,
        [FromQuery] int? teacherId = null, [FromQuery] int? academicYearId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId, examTypeId, classRoomId, subjectId, teacherId, academicYearId));

    // جدول امتحانات الطالب - حسب فصل الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<ExamScheduleDto>>> GetStudentExams(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();

        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);
        if (student == null) return NotFound();

        return Ok(await _service.GetByClassRoomIdsAsync(new List<int> { student.ClassRoomId }, schoolId));
    }

    // جدول امتحانات المعلم - حسب الفصول المعينة للمعلم
    [HttpGet("teacher")]
    public async Task<ActionResult<List<ExamScheduleDto>>> GetTeacherExams(int schoolId,
        [FromQuery] int? examTypeId = null, [FromQuery] int? subjectId = null,
        [FromQuery] int? classRoomId = null, [FromQuery] int? academicYearId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();

        var classRoomIds = await _assignmentRepo.Query()
            .Where(a => a.TeacherId == teacherId && a.SchoolId == schoolId)
            .Select(a => a.ClassRoomId)
            .Distinct()
            .ToListAsync();

        if (classRoomId.HasValue)
            classRoomIds = classRoomIds.Where(id => id == classRoomId.Value).ToList();

        var results = await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId);
        if (examTypeId.HasValue) results = results.Where(e => e.ExamTypeId == examTypeId.Value).ToList();
        if (subjectId.HasValue) results = results.Where(e => e.SubjectId == subjectId.Value).ToList();
        if (academicYearId.HasValue) results = results.Where(e => e.AcademicYearId == academicYearId.Value).ToList();
        return Ok(results);
    }

    // جدول امتحانات أبناء ولي الأمر - حسب فصول الأبناء
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<ExamScheduleDto>>> GetParentChildrenExams(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();

        var classRoomIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.ClassRoomId)
            .Distinct()
            .ToListAsync();

        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId));
    }
}
