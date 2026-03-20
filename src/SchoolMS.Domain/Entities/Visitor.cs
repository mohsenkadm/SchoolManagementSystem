using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class Visitor : BaseEntity
{
    public string VisitorName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? NationalId { get; set; }
    public string? Purpose { get; set; }
    public string? VisitingPerson { get; set; }
    public string? VisitingDepartment { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? BadgeNumber { get; set; }
    public string? Photo { get; set; }
    public int BranchId { get; set; }
    public VisitorStatus Status { get; set; }
    public string? Notes { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
