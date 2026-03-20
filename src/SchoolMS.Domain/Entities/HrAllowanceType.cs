using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrAllowanceType : BaseEntity
{
    public string AllowanceName { get; set; } = string.Empty;
    public string? AllowanceNameAr { get; set; }
    public string? AllowanceCode { get; set; }
    public AllowanceCalculation CalculationType { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
}
