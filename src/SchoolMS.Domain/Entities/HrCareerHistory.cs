namespace SchoolMS.Domain.Entities;

public class HrCareerHistory : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? EventType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? DecisionNumber { get; set; }
    public string? ProcessedBy { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
