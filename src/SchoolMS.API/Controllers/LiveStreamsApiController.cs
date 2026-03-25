using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolMS.API.Hubs;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using LiveStreamStatus = SchoolMS.Domain.Enums.LiveStreamStatus;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;
using UserType = SchoolMS.Domain.Enums.UserType;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة البث المباشر
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/live-streams")]
[Authorize]
public class LiveStreamsApiController : ControllerBase
{
    private readonly ILiveStreamService _service;
    private readonly ICourseService _courseService;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<StudentSubscription> _subscriptionRepo;
    private readonly IOneSignalNotificationService _pushService;
    private readonly IHubContext<ApiLiveStreamChatHub> _liveStreamHub;

    public LiveStreamsApiController(
        ILiveStreamService service,
        ICourseService courseService,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<StudentSubscription> subscriptionRepo,
        IOneSignalNotificationService pushService,
        IHubContext<ApiLiveStreamChatHub> liveStreamHub)
    {
        _service = service;
        _courseService = courseService;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _subscriptionRepo = subscriptionRepo;
        _pushService = pushService;
        _liveStreamHub = liveStreamHub;
    }

    private string UserTypeClaim => User.FindFirst("UserType")?.Value ?? "";
    private string UserName => User.FindFirst(ClaimTypes.Name)?.Value ?? "";

    [HttpGet]
    public async Task<ActionResult<List<LiveStreamDto>>> GetAll(
        [FromQuery] int? subjectId = null, [FromQuery] int? courseId = null,
        [FromQuery] int? teacherId = null)
    {
        List<LiveStreamDto> items;

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Ok(new List<LiveStreamDto>());
            items = await _service.GetByTeacherIdAsync(teacher.Id);
        }
        else if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Ok(new List<LiveStreamDto>());
            var subjectIds = await GetApprovedSubjectIdsAsync(student.Id);
            items = await _service.GetBySubjectIdsAsync(subjectIds);
        }
        else
        {
            items = await _service.GetAllAsync();
        }

        if (subjectId.HasValue) items = items.Where(l => l.SubjectId == subjectId.Value).ToList();
        if (courseId.HasValue) items = items.Where(l => l.CourseId == courseId.Value).ToList();
        if (teacherId.HasValue) items = items.Where(l => l.TeacherId == teacherId.Value).ToList();
        return Ok(items);
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<LiveStreamDto>>> GetByCourse(int courseId)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null || course.TeacherId != teacher.Id) return Forbid();
        }

        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Forbid();
            var subjectIds = await GetApprovedSubjectIdsAsync(student.Id);
            if (!subjectIds.Contains(course.SubjectId)) return Forbid();
        }

        return Ok(await _service.GetAllByCourseAsync(courseId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LiveStreamDto>> Get(int id)
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
    public async Task<ActionResult<LiveStreamDto>> Create(int schoolId, [FromBody] CreateLiveStreamDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            dto.TeacherId = teacher.Id;
        }

        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Live Stream", $"{dto.Title} is starting soon", new[] { "Student", "Teacher" }, schoolId);
        return Ok(result);
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] LiveStreamStatus status)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null || item.TeacherId != teacher.Id) return Forbid();
        }

        await _service.UpdateStatusAsync(id, status);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null || item.TeacherId != teacher.Id) return Forbid();
        }

        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("{id}/seen")]
    public async Task<IActionResult> MarkSeen(int id, [FromBody] int studentId)
    {
        await _service.MarkSeenAsync(id, studentId);
        return Ok(new { success = true });
    }

    // جلب تعليقات البث المباشر
    [HttpGet("{liveStreamId}/comments")]
    public async Task<ActionResult<List<LiveStreamCommentDto>>> GetComments(int schoolId, int liveStreamId)
        => Ok(await _service.GetCommentsByLiveStreamIdAsync(liveStreamId));

    // إضافة تعليق على البث المباشر مع إشعار SignalR
    [HttpPost("{liveStreamId}/comments")]
    public async Task<ActionResult<LiveStreamCommentDto>> AddComment(int schoolId, int liveStreamId, [FromBody] CreateLiveStreamCommentDto dto)
    {
        dto.LiveStreamId = liveStreamId;

        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Forbid();
            dto.StudentId = student.Id;
            dto.SenderName = student.FullName;
            dto.SenderType = "Student";
        }
        else if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            dto.TeacherId = teacher.Id;
            dto.SenderName = teacher.FullName;
            dto.SenderType = "Teacher";
        }
        else
        {
            return Forbid();
        }

        var result = await _service.AddCommentAsync(dto);

        // إرسال التعليق لجميع المشتركين عبر SignalR
        await _liveStreamHub.Clients.Group($"livestream-{liveStreamId}")
            .SendAsync("ReceiveLiveComment", result.SenderName, result.SenderType, dto.Comment, result.SentAt);

        return Ok(result);
    }

    private async Task<List<int>> GetApprovedSubjectIdsAsync(int studentId)
    {
        return await _subscriptionRepo.Query()
            .Where(s => s.StudentId == studentId
                && s.Status == SubscriptionStatus.Approved
                && s.EndDate >= DateTime.UtcNow)
            .Include(s => s.OnlineSubscriptionPlan)
            .Select(s => s.OnlineSubscriptionPlan.SubjectId)
            .Distinct()
            .ToListAsync();
    }

    private async Task<List<int>> GetApprovedSubjectIdsByTypeAsync(int studentId, SchoolMS.Domain.Enums.SubscriptionType type)
    {
        return await _subscriptionRepo.Query()
            .Where(s => s.StudentId == studentId
                && s.Status == SubscriptionStatus.Approved
                && s.EndDate >= DateTime.UtcNow
                && s.OnlineSubscriptionPlan.SubscriptionType == type)
            .Include(s => s.OnlineSubscriptionPlan)
            .Select(s => s.OnlineSubscriptionPlan.SubjectId)
            .Distinct()
            .ToListAsync();
    }
}
