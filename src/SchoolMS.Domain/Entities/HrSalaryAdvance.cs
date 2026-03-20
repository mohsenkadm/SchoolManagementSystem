using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrSalaryAdvance : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public DateTime RequestDate { get; set; }
    public string? Reason { get; set; }
    public AdvanceStatus Status { get; set; }
    public int DeductionMonths { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal DeductedSoFar { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime? FirstDeductionMonth { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual ICollection<HrAdvanceDeductionLog> DeductionLogs { get; set; } = new List<HrAdvanceDeductionLog>();
}

public class HrAdvanceDeductionLog : BaseEntity
{
    public int SalaryAdvanceId { get; set; }
    public int PayrollItemId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal DeductedAmount { get; set; }
    public decimal RemainingAfter { get; set; }

    public virtual HrSalaryAdvance SalaryAdvance { get; set; } = null!;
}
