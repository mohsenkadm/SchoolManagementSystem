using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrDeductionType : BaseEntity
{
    public string DeductionName { get; set; } = string.Empty;
    public string? DeductionNameAr { get; set; }
    public string? DeductionCode { get; set; }
    public DeductionCalculation CalculationType { get; set; }
    public decimal? DefaultValue { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}
