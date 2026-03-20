using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrEmployeeRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public EmployeeRequestType RequestType { get; set; }
    public string? Subject { get; set; }
    public string? Description { get; set; }
    public string? AttachmentPath { get; set; }
    public EmployeeRequestStatus Status { get; set; }
    public string? ProcessedBy { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string? Response { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
