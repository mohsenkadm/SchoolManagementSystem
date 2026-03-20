namespace SchoolMS.Domain.Entities;

public class VideoLike : BaseEntity
{
    public int CourseVideoId { get; set; }
    public int StudentId { get; set; }

    public virtual CourseVideo CourseVideo { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
