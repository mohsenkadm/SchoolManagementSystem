using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrPromotion : BaseEntity
{
    public int EmployeeId { get; set; }
    public PromotionType Type { get; set; }

    public int? FromJobTitleId { get; set; }
    public int? FromJobGradeId { get; set; }
    public int? FromJobGradeStepId { get; set; }
    public int? FromDepartmentId { get; set; }
    public int? FromBranchId { get; set; }
    public decimal? FromSalary { get; set; }

    public int? ToJobTitleId { get; set; }
    public int? ToJobGradeId { get; set; }
    public int? ToJobGradeStepId { get; set; }
    public int? ToDepartmentId { get; set; }
    public int? ToBranchId { get; set; }
    public decimal? ToSalary { get; set; }

    public decimal? SalaryIncrease { get; set; }
    public decimal? SalaryIncreasePercentage { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string? Reason { get; set; }
    public string? DecisionNumber { get; set; }
    public string? AttachmentPath { get; set; }
    public HrPromotionStatus Status { get; set; }
    public string? RequestedBy { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
