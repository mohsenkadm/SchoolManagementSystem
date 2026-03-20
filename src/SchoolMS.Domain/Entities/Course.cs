namespace SchoolMS.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public string? ThumbnailImage { get; set; }
    public string? BackgroundImage { get; set; }
    public bool IsPublished { get; set; }

    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ICollection<CourseVideo> Videos { get; set; } = new List<CourseVideo>();
    public virtual ICollection<LiveStream> LiveStreams { get; set; } = new List<LiveStream>();
}
