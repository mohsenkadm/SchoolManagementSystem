using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المحادثات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/chat")]
[Authorize]
public class ChatApiController : ControllerBase
{
    private readonly IChatService _service;
    private readonly IOneSignalNotificationService _pushService;
    public ChatApiController(IChatService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    // جلب جميع غرف المحادثة للمدرسة
    [HttpGet("rooms")]
    public async Task<ActionResult<List<ChatRoomDto>>> GetRooms(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetRoomsBySchoolIdAsync(schoolId, branchId));

    // إنشاء غرفة محادثة جديدة
    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomDto>> CreateRoom(int schoolId, [FromBody] ChatRoomDto dto)
    { var r = await _service.CreateRoomAsync(dto); await _pushService.SendToSchoolAsync("New Chat Room", $"{dto.RoomName} has been created", schoolId); return Ok(r); }

    // جلب رسائل غرفة محادثة معينة
    [HttpGet("rooms/{roomId}/messages")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetMessages(int schoolId, int roomId)
        => Ok(await _service.GetMessagesAsync(roomId));

    // إرسال رسالة
    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(int schoolId, [FromBody] ChatMessageDto dto)
        => Ok(await _service.SendMessageAsync(dto));
}
