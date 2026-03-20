using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class LiveStream : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int? CourseId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? CloudflareStreamId { get; set; }
    public string? StreamUrl { get; set; }
    public LiveStreamStatus Status { get; set; }
    public int SeenCount { get; set; }

    public virtual Course? Course { get; set; }
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ICollection<LiveStreamComment> Comments { get; set; } = new List<LiveStreamComment>();
    public virtual ICollection<LiveStreamSeen> Seens { get; set; } = new List<LiveStreamSeen>();
}
