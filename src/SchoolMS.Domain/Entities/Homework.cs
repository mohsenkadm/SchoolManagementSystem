namespace SchoolMS.Domain.Entities;

public class Homework : BaseEntity
{
    public int TeacherId { get; set; }
    public int ClassRoomId { get; set; }
    public int SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public int AcademicYearId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual ICollection<HomeworkAttachment> Attachments { get; set; } = new List<HomeworkAttachment>();
    public virtual ICollection<HomeworkComment> Comments { get; set; } = new List<HomeworkComment>();
}
