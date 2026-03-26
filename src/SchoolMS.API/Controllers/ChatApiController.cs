using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolMS.API.Hubs;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Application.Settings;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using UserType = SchoolMS.Domain.Enums.UserType;

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
    private readonly IHubContext<ApiChatHub> _chatHub;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;

    public ChatApiController(
        IChatService service,
        IOneSignalNotificationService pushService,
        IHubContext<ApiChatHub> chatHub,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo)
    {
        _service = service;
        _pushService = pushService;
        _chatHub = chatHub;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
    }

    private string UserTypeClaim => User.FindFirst("UserType")?.Value ?? "";
    private string UserName => User.FindFirst(ClaimTypes.Name)?.Value ?? "";

    // جلب جميع غرف المحادثة للمدرسة
    [HttpGet("rooms")]
    public async Task<ActionResult<List<ChatRoomDto>>> GetRooms(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetRoomsBySchoolIdAsync(schoolId, branchId));

    // جلب غرف المحادثة للطالب (حسب الكلاس والفرع والمدرسة)
    [HttpGet("rooms/student/{studentId}")]
    public async Task<ActionResult<List<ChatRoomDto>>> GetRoomsByStudent(int schoolId, int studentId)
        => Ok(await _service.GetRoomsByStudentAsync(studentId));

    // جلب غرف المحادثة للمدرس (حسب الكلاسات المرتبطة به)
    [HttpGet("rooms/teacher/{teacherId}")]
    public async Task<ActionResult<List<ChatRoomDto>>> GetRoomsByTeacher(int schoolId, int teacherId)
        => Ok(await _service.GetRoomsByTeacherAsync(teacherId));

    // إنشاء غرفة محادثة جديدة (فقط المدرس أو الأدمن)
    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomDto>> CreateRoom(int schoolId, [FromBody] ChatRoomDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            dto.TeacherId = teacher.Id;
        }

        var r = await _service.CreateRoomAsync(dto);
        await _pushService.SendToSchoolAsync("New Chat Room", $"{dto.RoomName} has been created", schoolId);
        return Ok(r);
    }

    // جلب رسائل غرفة محادثة معينة
    [HttpGet("rooms/{roomId}/messages")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetMessages(int schoolId, int roomId)
        => Ok(await _service.GetMessagesAsync(roomId));

    // إرسال رسالة مع إشعار SignalR
    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage(int schoolId, [FromBody] ChatMessageDto dto)
    {
        var result = await _service.SendMessageAsync(dto);

        // إشعار SignalR لجميع المتصلين في الغرفة
        await _chatHub.Clients.Group(dto.ChatRoomId.ToString())
            .SendAsync("ReceiveMessage", result.SenderName, result.Message,
                result.FileUrl, result.FileType, result.SentAt);

        return Ok(result);
    }

    // رفع ملف مرفق (صورة أو PDF)
    [HttpPost("messages/upload")]
    [RequestSizeLimit(20_000_000)]
    public async Task<ActionResult<ChatMessageDto>> SendMessageWithFile(
        int schoolId,
        [FromForm] int chatRoomId,
        [FromForm] int senderId,
        [FromForm] string senderType,
        [FromForm] string senderName,
        [FromForm] string? message,
        IFormFile? file)
    {
        string? fileUrl = null;
        string? fileType = null;

        if (file != null && file.Length > 0)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Only images and PDF files are allowed." });

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "chat");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            fileUrl = AppUrlSettings.BuildApiUrl($"/uploads/chat/{fileName}");
            fileType = file.ContentType;
        }

        var dto = new ChatMessageDto
        {
            ChatRoomId = chatRoomId,
            SenderId = senderId,
            SenderType = senderType,
            SenderName = senderName,
            Message = message ?? string.Empty,
            FileUrl = fileUrl,
            FileType = fileType
        };

        var result = await _service.SendMessageAsync(dto);

        await _chatHub.Clients.Group(chatRoomId.ToString())
            .SendAsync("ReceiveMessage", result.SenderName, result.Message,
                result.FileUrl, result.FileType, result.SentAt);

        return Ok(result);
    }
}
