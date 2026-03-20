using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class QuizQuestion : BaseEntity
{
    public int QuizGroupId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public string? Options { get; set; }
    public string? CorrectAnswer { get; set; }
    public int Points { get; set; }

    public virtual QuizGroup QuizGroup { get; set; } = null!;
    public virtual ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
