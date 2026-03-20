namespace SchoolMS.Domain.Entities;

public class HrWorkShift : BaseEntity
{
    public string ShiftName { get; set; } = string.Empty;
    public string? ShiftNameAr { get; set; }
    public string? ShiftCode { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public int BreakDurationMinutes { get; set; }
    public decimal TotalWorkHours { get; set; }
    public int GracePeriodMinutes { get; set; }
    public int EarlyLeaveGraceMinutes { get; set; }
    public bool IsFlexible { get; set; }
    public TimeSpan? FlexStartFrom { get; set; }
    public TimeSpan? FlexStartTo { get; set; }
    public string? WorkingDays { get; set; }
    public bool IsNightShift { get; set; }
    public bool IsDefault { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<HrEmployee> Employees { get; set; } = new List<HrEmployee>();
}
