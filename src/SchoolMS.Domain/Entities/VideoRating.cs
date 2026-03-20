namespace SchoolMS.Domain.Entities;

public class VideoRating : BaseEntity
{
    public int CourseVideoId { get; set; }
    public int StudentId { get; set; }
    public int Rating { get; set; }

    public virtual CourseVideo CourseVideo { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
