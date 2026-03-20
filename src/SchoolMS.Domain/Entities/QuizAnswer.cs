namespace SchoolMS.Domain.Entities;

public class QuizAnswer : BaseEntity
{
    public int QuizQuestionId { get; set; }
    public int StudentId { get; set; }
    public string? Answer { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }

    public virtual QuizQuestion QuizQuestion { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
