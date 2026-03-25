namespace SchoolMS.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public int ChatRoomId { get; set; }
    public int SenderId { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;
}
