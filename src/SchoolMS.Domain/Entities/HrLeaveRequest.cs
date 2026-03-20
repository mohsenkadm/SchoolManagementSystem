using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrLeaveRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public bool IsHalfDay { get; set; }
    public string? HalfDayPeriod { get; set; }
    public string? Reason { get; set; }
    public string? AttachmentPath { get; set; }
    public HrLeaveStatus Status { get; set; }
    public int? SubstituteEmployeeId { get; set; }
    public string? ApprovedByManager { get; set; }
    public DateTime? ManagerApprovalDate { get; set; }
    public string? ApprovedByHR { get; set; }
    public DateTime? HrApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public string? EmergencyContact { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrLeaveType LeaveType { get; set; } = null!;
}
