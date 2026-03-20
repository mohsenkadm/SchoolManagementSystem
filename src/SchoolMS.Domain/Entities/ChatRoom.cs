using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class ChatRoom : BaseEntity
{
    public string RoomName { get; set; } = string.Empty;
    public ChatRoomType Type { get; set; }
    public int? BranchId { get; set; }
    public int? ClassRoomId { get; set; }
    public int? SubjectId { get; set; }

    public virtual Branch? Branch { get; set; }
    public virtual ClassRoom? ClassRoom { get; set; }
    public virtual Subject? Subject { get; set; }
    public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
