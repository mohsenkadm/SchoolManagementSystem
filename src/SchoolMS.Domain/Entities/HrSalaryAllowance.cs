using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrSalaryAllowance : BaseEntity
{
    public int SalaryDetailId { get; set; }
    public int AllowanceTypeId { get; set; }
    public AllowanceCalculation CalculationType { get; set; }
    public decimal Value { get; set; }
    public decimal CalculatedAmount { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }

    public virtual HrSalaryDetail SalaryDetail { get; set; } = null!;
    public virtual HrAllowanceType AllowanceType { get; set; } = null!;
}
