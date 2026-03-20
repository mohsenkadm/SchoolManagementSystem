namespace SchoolMS.Domain.Entities;

public class School : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool OnlinePlatformEnabled { get; set; }
    public bool IsHrModuleEnabled { get; set; }

    // HR Settings
    public bool HrRequireApprovalForLeaves { get; set; }
    public bool HrAutoCalculateOvertime { get; set; }
    public bool HrAutoDeductAbsence { get; set; }
    public bool HrEnableFingerprintIntegration { get; set; }
    public bool HrEnableSelfService { get; set; }
    public int HrMaxOvertimeHoursPerMonth { get; set; }
    public decimal HrOvertimeRateMultiplier { get; set; }
    public int HrLateGracePeriodMinutes { get; set; }
    public TimeSpan HrWorkDayStart { get; set; }
    public TimeSpan HrWorkDayEnd { get; set; }
    public int HrWorkingDaysPerMonth { get; set; }
    public decimal HrAbsenceDeductionPerDay { get; set; }
    public string? HrAbsenceDeductionType { get; set; }
    public decimal HrLateDeductionPerMinute { get; set; }
    public decimal HrEarlyLeaveDeductionPerMinute { get; set; }
    public string? HrSalaryCalculationMethod { get; set; }

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public virtual ICollection<SchoolSubscription> SchoolSubscriptions { get; set; } = new List<SchoolSubscription>();
}
