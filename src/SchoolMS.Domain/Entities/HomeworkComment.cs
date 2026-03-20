namespace SchoolMS.Domain.Entities;

public class HomeworkComment : BaseEntity
{
    public int HomeworkId { get; set; }
    public int StudentId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }

    public virtual Homework Homework { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
