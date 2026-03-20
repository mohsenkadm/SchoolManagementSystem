using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationTarget Target { get; set; }
    public int? TargetPersonId { get; set; }
    public PersonType? TargetPersonType { get; set; }
    public int? TargetClassRoomId { get; set; }
    public int? TargetBranchId { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
}
