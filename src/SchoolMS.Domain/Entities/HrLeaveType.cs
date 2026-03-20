namespace SchoolMS.Domain.Entities;

public class HrLeaveType : BaseEntity
{
    public string LeaveTypeName { get; set; } = string.Empty;
    public string? LeaveTypeNameAr { get; set; }
    public string? LeaveCode { get; set; }
    public int DefaultDaysPerYear { get; set; }
    public bool IsPaid { get; set; }
    public bool RequiresApproval { get; set; }
    public bool RequiresDocument { get; set; }
    public bool DeductsFromSalary { get; set; }
    public decimal? DeductionPerDay { get; set; }
    public bool AllowHalfDay { get; set; }
    public bool AllowNegativeBalance { get; set; }
    public int? MaxConsecutiveDays { get; set; }
    public int? MinAdvanceNoticeDays { get; set; }
    public bool CarryForward { get; set; }
    public int? MaxCarryForwardDays { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public string? ApplicableFor { get; set; }
    public bool IsActive { get; set; }
}
