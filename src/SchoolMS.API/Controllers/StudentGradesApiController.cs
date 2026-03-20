using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة درجات الطلاب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/student-grades")]
[Authorize]
public class StudentGradesApiController : ControllerBase
{
    private readonly IStudentGradeService _service;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<TeacherAssignment> _assignmentRepo;

    public StudentGradesApiController(
        IStudentGradeService service,
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

    // جلب درجات الطلاب للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<StudentGradeDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    // درجات الطالب - حسب فصل الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<StudentGradeDto>>> GetStudentGrades(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);
        if (student == null) return NotFound();
        return Ok(await _service.GetByClassRoomIdsAsync(new List<int> { student.ClassRoomId }, schoolId));
    }

    // درجات المعلم - حسب الفصول المعينة للمعلم
    [HttpGet("teacher")]
    public async Task<ActionResult<List<StudentGradeDto>>> GetTeacherGrades(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();
        var classRoomIds = await _assignmentRepo.Query()
            .Where(a => a.TeacherId == teacherId && a.SchoolId == schoolId)
            .Select(a => a.ClassRoomId).Distinct().ToListAsync();
        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId));
    }

    // درجات أبناء ولي الأمر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<StudentGradeDto>>> GetParentChildrenGrades(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        var classRoomIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.ClassRoomId).Distinct().ToListAsync();
        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId));
    }
}
