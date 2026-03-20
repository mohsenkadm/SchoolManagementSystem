namespace SchoolMS.Domain.Entities;

public class LiveStreamSeen : BaseEntity
{
    public int LiveStreamId { get; set; }
    public int StudentId { get; set; }
    public DateTime SeenAt { get; set; }

    public virtual LiveStream LiveStream { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
