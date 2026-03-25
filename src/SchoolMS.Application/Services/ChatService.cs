using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ChatService : IChatService
{
    private readonly IRepository<ChatRoom> _roomRepo;
    private readonly IRepository<ChatMessage> _messageRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<TeacherAssignment> _assignmentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(
        IRepository<ChatRoom> roomRepo,
        IRepository<ChatMessage> messageRepo,
        IRepository<Student> studentRepo,
        IRepository<TeacherAssignment> assignmentRepo,
        IUnitOfWork unitOfWork)
    {
        _roomRepo = roomRepo;
        _messageRepo = messageRepo;
        _studentRepo = studentRepo;
        _assignmentRepo = assignmentRepo;
        _unitOfWork = unitOfWork;
    }

    private static ChatRoomDto MapRoom(ChatRoom r) => new()
    {
        Id = r.Id, RoomName = r.RoomName, Type = r.Type, SchoolId = r.SchoolId,
        BranchId = r.BranchId, BranchName = r.Branch?.Name,
        ClassRoomId = r.ClassRoomId,
        ClassRoomName = r.ClassRoom != null ? $"{r.ClassRoom.Grade?.GradeName} - {r.ClassRoom.Division?.DivisionName}" : null,
        SubjectId = r.SubjectId, SubjectName = r.Subject?.SubjectName,
        TeacherId = r.TeacherId, TeacherName = r.Teacher?.FullName
    };

    private IQueryable<ChatRoom> RoomsWithIncludes() => _roomRepo.Query()
        .Include(r => r.Branch)
        .Include(r => r.ClassRoom).ThenInclude(c => c!.Grade)
        .Include(r => r.ClassRoom).ThenInclude(c => c!.Division)
        .Include(r => r.Subject)
        .Include(r => r.Teacher);

    public async Task<List<ChatRoomDto>> GetAllRoomsAsync()
    {
        var items = await RoomsWithIncludes().ToListAsync();
        return items.Select(MapRoom).ToList();
    }

    public async Task<List<ChatRoomDto>> GetRoomsByBranchIdAsync(int branchId)
    {
        var items = await RoomsWithIncludes().Where(r => r.BranchId == branchId).ToListAsync();
        return items.Select(MapRoom).ToList();
    }

    public async Task<List<ChatRoomDto>> GetRoomsBySchoolIdAsync(int schoolId, int? branchId = null)
    {
        var query = RoomsWithIncludes().Where(r => r.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(r => r.BranchId == branchId.Value);
        var items = await query.ToListAsync();
        return items.Select(MapRoom).ToList();
    }

    public async Task<ChatRoomDto> CreateRoomAsync(ChatRoomDto dto)
    {
        var entity = new ChatRoom
        {
            RoomName = dto.RoomName, Type = dto.Type,
            BranchId = dto.BranchId, ClassRoomId = dto.ClassRoomId,
            SubjectId = dto.SubjectId, SchoolId = dto.SchoolId,
            TeacherId = dto.TeacherId
        };
        await _roomRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id;
        return dto;
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(int roomId)
    {
        var items = await _messageRepo.Query()
            .Where(m => m.ChatRoomId == roomId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
        return items.Select(m => new ChatMessageDto
        {
            Id = m.Id, ChatRoomId = m.ChatRoomId, SenderId = m.SenderId,
            SenderType = m.SenderType, SenderName = m.SenderName,
            Message = m.Message, FileUrl = m.FileUrl, FileType = m.FileType,
            SentAt = m.SentAt, IsRead = m.IsRead
        }).ToList();
    }

    public async Task<ChatMessageDto> SendMessageAsync(ChatMessageDto dto)
    {
        var entity = new ChatMessage
        {
            ChatRoomId = dto.ChatRoomId, SenderId = dto.SenderId,
            SenderType = dto.SenderType ?? string.Empty,
            SenderName = dto.SenderName ?? string.Empty,
            Message = dto.Message,
            FileUrl = dto.FileUrl, FileType = dto.FileType,
            SentAt = DateTime.UtcNow,
            SchoolId = (await _roomRepo.GetByIdAsync(dto.ChatRoomId))?.SchoolId ?? 0
        };
        await _messageRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id;
        dto.SentAt = entity.SentAt;
        return dto;
    }

    public async Task<List<ChatRoomDto>> GetRoomsByStudentAsync(int studentId)
    {
        var student = await _studentRepo.Query()
            .FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null) return new List<ChatRoomDto>();

        var items = await RoomsWithIncludes()
            .Where(r => r.SchoolId == student.SchoolId
                && r.BranchId == student.BranchId
                && (r.ClassRoomId == null || r.ClassRoomId == student.ClassRoomId))
            .ToListAsync();
        return items.Select(MapRoom).ToList();
    }

    public async Task<List<ChatRoomDto>> GetRoomsByTeacherAsync(int teacherId)
    {
        var classRoomIds = await _assignmentRepo.Query()
            .Where(a => a.TeacherId == teacherId)
            .Select(a => a.ClassRoomId)
            .Distinct()
            .ToListAsync();

        var items = await RoomsWithIncludes()
            .Where(r => r.TeacherId == teacherId
                || (r.ClassRoomId.HasValue && classRoomIds.Contains(r.ClassRoomId.Value)))
            .ToListAsync();
        return items.Select(MapRoom).ToList();
    }
}

