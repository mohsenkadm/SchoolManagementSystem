using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class Complaint : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;
    public int? PersonId { get; set; }
    public PersonType? PersonType { get; set; }
    public string? PersonName { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ComplaintCategory Category { get; set; }
    public ComplaintPriority Priority { get; set; }
    public ComplaintStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public string? Resolution { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsAnonymous { get; set; }
}
