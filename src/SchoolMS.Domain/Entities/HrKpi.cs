using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrKpi : BaseEntity
{
    public int EmployeeId { get; set; }
    public string KpiName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? MeasurementUnit { get; set; }
    public decimal TargetValue { get; set; }
    public decimal ActualValue { get; set; }
    public decimal AchievementPercentage { get; set; }
    public int? PerformanceCycleId { get; set; }
    public KpiStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrPerformanceCycle? PerformanceCycle { get; set; }
}
