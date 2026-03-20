using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrViolationType : BaseEntity
{
    public string ViolationName { get; set; } = string.Empty;
    public string? ViolationNameAr { get; set; }
    public string? Description { get; set; }
    public ViolationSeverity Severity { get; set; }
    public string? DefaultAction { get; set; }
    public decimal? DefaultPenaltyAmount { get; set; }
    public int? DefaultSuspensionDays { get; set; }
    public bool IsActive { get; set; }
}
