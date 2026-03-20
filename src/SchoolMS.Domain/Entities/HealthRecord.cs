namespace SchoolMS.Domain.Entities;

public class HealthRecord : BaseEntity
{
    public int StudentId { get; set; }
    public DateTime RecordDate { get; set; }
    public string? RecordType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DoctorName { get; set; }
    public string? Prescription { get; set; }
    public string? AttachmentUrl { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public bool NotifyParent { get; set; }

    public virtual Student Student { get; set; } = null!;
}
