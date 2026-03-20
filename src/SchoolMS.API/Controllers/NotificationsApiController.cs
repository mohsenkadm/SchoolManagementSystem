using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

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

    // جلب جميع الإشعارات للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    // تحديد الإشعار كمرسل
    [HttpPost("{id}/send")]
    public async Task<IActionResult> MarkAsSent(int schoolId, int id)
    {
        await _service.MarkAsSentAsync(id);
        await _pushService.SendToSchoolAsync("New Notification", "You have a new notification", schoolId);
        return Ok();
    }
}
