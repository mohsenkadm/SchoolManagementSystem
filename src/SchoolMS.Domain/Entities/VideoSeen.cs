namespace SchoolMS.Domain.Entities;

public class VideoSeen : BaseEntity
{
    public int CourseVideoId { get; set; }
    public int StudentId { get; set; }
    public DateTime SeenAt { get; set; }

    public virtual CourseVideo CourseVideo { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
