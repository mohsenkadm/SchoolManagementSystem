namespace SchoolMS.Domain.Entities;

public class HomeworkAttachment : BaseEntity
{
    public int HomeworkId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileType { get; set; }

    public virtual Homework Homework { get; set; } = null!;
}
