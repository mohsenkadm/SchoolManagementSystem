namespace SchoolMS.Domain.Entities;

public class HrLeaveBalance : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public decimal TotalEntitlement { get; set; }
    public decimal CarriedForward { get; set; }
    public decimal TotalAvailable { get; set; }
    public decimal Used { get; set; }
    public decimal Pending { get; set; }
    public decimal Remaining { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrLeaveType LeaveType { get; set; } = null!;
}
