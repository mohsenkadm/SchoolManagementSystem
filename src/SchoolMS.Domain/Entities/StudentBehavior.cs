using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class StudentBehavior : BaseEntity
{
    public int StudentId { get; set; }
    public BehaviorType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Points { get; set; }
    public string? ActionTaken { get; set; }
    public string? RecordedBy { get; set; }
    public DateTime IncidentDate { get; set; }
    public bool NotifyParent { get; set; }
    public string? ParentResponse { get; set; }
    public int? AcademicYearId { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual AcademicYear? AcademicYear { get; set; }
}
