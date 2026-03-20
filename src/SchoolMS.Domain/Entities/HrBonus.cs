using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrBonus : BaseEntity
{
    public int EmployeeId { get; set; }
    public string? BonusType { get; set; }
    public decimal Amount { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public string? Reason { get; set; }
    public BonusStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public bool IncludeInPayroll { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
