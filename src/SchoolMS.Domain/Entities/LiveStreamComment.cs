namespace SchoolMS.Domain.Entities;

public class LiveStreamComment : BaseEntity
{
    public int LiveStreamId { get; set; }
    public int StudentId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    public virtual LiveStream LiveStream { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
