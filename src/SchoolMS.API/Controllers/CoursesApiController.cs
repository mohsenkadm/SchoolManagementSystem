using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;
using SubscriptionType = SchoolMS.Domain.Enums.SubscriptionType;
using UserType = SchoolMS.Domain.Enums.UserType;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الكورسات التعليمية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/courses")]
[Authorize]
public class CoursesApiController : ControllerBase
{
    private readonly ICourseService _service;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<StudentSubscription> _subscriptionRepo;
    private readonly IOneSignalNotificationService _pushService;

    public CoursesApiController(
        ICourseService service,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<StudentSubscription> subscriptionRepo,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _subscriptionRepo = subscriptionRepo;
        _pushService = pushService;
    }

    private string UserTypeClaim => User.FindFirst("UserType")?.Value ?? "";
    private string UserName => User.FindFirst(ClaimTypes.Name)?.Value ?? "";

    [HttpGet]
    public async Task<ActionResult<List<CourseDto>>> GetAll(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] int? teacherId = null)
    {
        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Ok(new List<CourseDto>());
            var courses = await _service.GetByTeacherIdAsync(teacher.Id);
            if (subjectId.HasValue)
                courses = courses.Where(c => c.SubjectId == subjectId.Value).ToList();
            return Ok(courses);
        }

        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Ok(new List<CourseDto>());
            var subjectIds = await GetApprovedSubjectIdsAsync(student.Id);
            var courses = await _service.GetBySubjectIdsAsync(subjectIds);
            if (subjectId.HasValue)
                courses = courses.Where(c => c.SubjectId == subjectId.Value).ToList();
            if (teacherId.HasValue)
                courses = courses.Where(c => c.TeacherId == teacherId.Value).ToList();
            return Ok(courses);
        }

        var all = await _service.GetAllAsync();
        if (subjectId.HasValue)
            all = all.Where(c => c.SubjectId == subjectId.Value).ToList();
        if (teacherId.HasValue)
            all = all.Where(c => c.TeacherId == teacherId.Value).ToList();
        return Ok(all);
    }

    // جلب أفضل 10 كورسات للطلاب حسب عدد الاشتراكات
    [HttpGet("top")]
    public async Task<ActionResult<List<CourseDto>>> GetTopCourses(int schoolId, [FromQuery] int count = 10)
        => Ok(await _service.GetTopCoursesAsync(schoolId, count));

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null || item.TeacherId != teacher.Id) return Forbid();
        }

        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Forbid();
            var subjectIds = await GetApprovedSubjectIdsAsync(student.Id);
            if (!subjectIds.Contains(item.SubjectId)) return Forbid();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CourseDto>> Create(int schoolId, [FromBody] CreateCourseDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Course Available", $"{dto.Title} has been added", new[] { "Student", "Teacher" }, schoolId); return Ok(r); }

    [HttpPut]
    public async Task<ActionResult<CourseDto>> Update(int schoolId, [FromBody] CourseDto dto)
    { var r = await _service.UpdateAsync(dto); await _pushService.SendToPersonTypesAsync("Course Updated", $"{dto.Title} has been updated", new[] { "Student", "Teacher" }, schoolId); return Ok(r); }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id) { await _service.DeleteAsync(id); return Ok(); }

    private async Task<List<int>> GetApprovedSubjectIdsAsync(int studentId)
    {
        return await _subscriptionRepo.Query()
            .Where(s => s.StudentId == studentId
                && s.Status == SubscriptionStatus.Approved
                && s.EndDate >= DateTime.UtcNow
                && s.OnlineSubscriptionPlan.SubscriptionType == SubscriptionType.Course)
            .Include(s => s.OnlineSubscriptionPlan)
            .Select(s => s.OnlineSubscriptionPlan.SubjectId)
            .Distinct()
            .ToListAsync();
    }
}
