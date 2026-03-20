using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrSalaryDeduction : BaseEntity
{
    public int SalaryDetailId { get; set; }
    public int DeductionTypeId { get; set; }
    public DeductionCalculation CalculationType { get; set; }
    public decimal Value { get; set; }
    public decimal CalculatedAmount { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }

    public virtual HrSalaryDetail SalaryDetail { get; set; } = null!;
    public virtual HrDeductionType DeductionType { get; set; } = null!;
}
