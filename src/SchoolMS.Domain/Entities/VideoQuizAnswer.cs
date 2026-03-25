namespace SchoolMS.Domain.Entities;

public class VideoQuizAnswer : BaseEntity
{
    public int VideoQuizQuestionId { get; set; }
    public int StudentId { get; set; }
    public string? SelectedAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AnsweredAt { get; set; }

    public virtual VideoQuizQuestion VideoQuizQuestion { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
