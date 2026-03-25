using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الإشعارات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/notifications")]
[Authorize]
public class NotificationsApiController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IOneSignalNotificationService _pushService;
    public NotificationsApiController(INotificationService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());

    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // جلب جميع الإشعارات للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    // جلب إشعارات الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<NotificationDto>>> GetStudentNotifications(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var personId = GetPersonIdFromToken();
        var all = await _service.GetBySchoolIdAsync(schoolId);
        var filtered = all.Where(n =>
            n.Target == NotificationTarget.All
            || (n.Target == NotificationTarget.Individual && n.TargetPersonId == personId)
        ).ToList();
        return Ok(filtered);
    }

    // جلب إشعارات المعلم
    [HttpGet("teacher")]
    public async Task<ActionResult<List<NotificationDto>>> GetTeacherNotifications(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var personId = GetPersonIdFromToken();
        var all = await _service.GetBySchoolIdAsync(schoolId);
        var filtered = all.Where(n =>
            n.Target == NotificationTarget.All
            || (n.Target == NotificationTarget.Individual && n.TargetPersonId == personId)
        ).ToList();
        return Ok(filtered);
    }

    // جلب إشعارات ولي الأمر
    [HttpGet("parent")]
    public async Task<ActionResult<List<NotificationDto>>> GetParentNotifications(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var personId = GetPersonIdFromToken();
        var all = await _service.GetBySchoolIdAsync(schoolId);
        var filtered = all.Where(n =>
            n.Target == NotificationTarget.All
            || (n.Target == NotificationTarget.Individual && n.TargetPersonId == personId)
        ).ToList();
        return Ok(filtered);
    }

    // جلب إشعارات الموظفين
    [HttpGet("staff")]
    public async Task<ActionResult<List<NotificationDto>>> GetStaffNotifications(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Staff") return Forbid();
        var personId = GetPersonIdFromToken();
        var all = await _service.GetBySchoolIdAsync(schoolId);
        var filtered = all.Where(n =>
            n.Target == NotificationTarget.All
            || (n.Target == NotificationTarget.Individual && n.TargetPersonId == personId)
        ).ToList();
        return Ok(filtered);
    }

    // تحديد الإشعار كمرسل
    [HttpPost("{id}/send")]
    public async Task<IActionResult> MarkAsSent(int schoolId, int id)
    {
        await _service.MarkAsSentAsync(id);
        await _pushService.SendToSchoolAsync("New Notification", "You have a new notification", schoolId);
        return Ok();
    }
}
