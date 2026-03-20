namespace SchoolMS.Domain.Entities;

public class VideoComment : BaseEntity
{
    public int CourseVideoId { get; set; }
    public int StudentId { get; set; }
    public string Comment { get; set; } = string.Empty;

    public virtual CourseVideo CourseVideo { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
