namespace SchoolMS.Domain.Entities;

public class HrClearanceItem : BaseEntity
{
    public int EndOfServiceId { get; set; }
    public string? Department { get; set; }
    public string? ItemDescription { get; set; }
    public bool IsCompleted { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }

    public virtual HrEndOfService EndOfService { get; set; } = null!;
}
