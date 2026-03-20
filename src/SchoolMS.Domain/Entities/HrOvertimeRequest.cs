using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrOvertimeRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime OvertimeDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal Hours { get; set; }
    public decimal RateMultiplier { get; set; }
    public decimal CalculatedAmount { get; set; }
    public string? Reason { get; set; }
    public OvertimeStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsFromAttendance { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
