using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrEndOfService : BaseEntity
{
    public int EmployeeId { get; set; }
    public EndOfServiceType Type { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? LastWorkingDay { get; set; }
    public string? Reason { get; set; }
    public string? DetailedReason { get; set; }
    public int NoticePeriodDays { get; set; }
    public bool IsNoticePeriodServed { get; set; }

    public int TotalServiceYears { get; set; }
    public int TotalServiceMonths { get; set; }
    public int TotalServiceDays { get; set; }
    public decimal LastBaseSalary { get; set; }
    public decimal EndOfServiceBenefit { get; set; }
    public decimal UnusedLeaveCompensation { get; set; }
    public decimal PendingBonuses { get; set; }
    public decimal PendingAllowances { get; set; }
    public decimal TotalEntitlements { get; set; }

    public decimal PendingAdvances { get; set; }
    public decimal PendingLoans { get; set; }
    public decimal PendingPenalties { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions { get; set; }

    public decimal FinalSettlementAmount { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }

    public bool AssetsClearance { get; set; }
    public bool FinanceClearance { get; set; }
    public bool ItClearance { get; set; }
    public bool HrClearance { get; set; }
    public bool AllClearancesCompleted { get; set; }

    public EndOfServiceStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }
    public string? ExitInterviewNotes { get; set; }
    public int? ExitInterviewRating { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual ICollection<HrClearanceItem> ClearanceItems { get; set; } = new List<HrClearanceItem>();
}
