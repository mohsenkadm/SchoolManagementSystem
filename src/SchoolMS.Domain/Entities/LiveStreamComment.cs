namespace SchoolMS.Domain.Entities;

public class LiveStreamComment : BaseEntity
{
    public int LiveStreamId { get; set; }
    public int? StudentId { get; set; }
    public int? TeacherId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderType { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    public virtual LiveStream LiveStream { get; set; } = null!;
    public virtual Student? Student { get; set; }
    public virtual Teacher? Teacher { get; set; }
}
