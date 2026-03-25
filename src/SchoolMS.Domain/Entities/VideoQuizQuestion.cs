namespace SchoolMS.Domain.Entities;

public class VideoQuizQuestion : BaseEntity
{
    public int CourseVideoId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    /// <summary>JSON array of option strings, e.g. ["Option A","Option B","Option C","Option D"]</summary>
    public string Options { get; set; } = "[]";
    public string CorrectAnswer { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public virtual CourseVideo CourseVideo { get; set; } = null!;
    public virtual ICollection<VideoQuizAnswer> Answers { get; set; } = new List<VideoQuizAnswer>();
}
