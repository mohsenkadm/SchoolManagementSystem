using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IChatService
{
    Task<List<ChatRoomDto>> GetAllRoomsAsync();
    Task<List<ChatRoomDto>> GetRoomsByBranchIdAsync(int branchId);
    Task<List<ChatRoomDto>> GetRoomsBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<List<ChatRoomDto>> GetRoomsByStudentAsync(int studentId);
    Task<List<ChatRoomDto>> GetRoomsByTeacherAsync(int teacherId);
    Task<ChatRoomDto> CreateRoomAsync(ChatRoomDto dto);
    Task<List<ChatMessageDto>> GetMessagesAsync(int roomId);
    Task<ChatMessageDto> SendMessageAsync(ChatMessageDto dto);
}

