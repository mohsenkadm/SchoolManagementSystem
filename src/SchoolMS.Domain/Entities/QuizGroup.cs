namespace SchoolMS.Domain.Entities;

public class QuizGroup : BaseEntity
{
    public string GroupName { get; set; } = string.Empty;
    public int ClassRoomId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int AcademicYearId { get; set; }

    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}
