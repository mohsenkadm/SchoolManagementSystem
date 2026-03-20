using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class SchoolEvent : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }
    public EventType EventCategory { get; set; }
    public string? Color { get; set; }
    public bool IsAllDay { get; set; }
    public bool NotifyAll { get; set; }
    public int? BranchId { get; set; }
    public string? AttachmentUrl { get; set; }
    public string? OrganizerName { get; set; }

    public virtual Branch? Branch { get; set; }
}
