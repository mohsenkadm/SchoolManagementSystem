using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrDailyAttendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int? WorkShiftId { get; set; }

    public TimeSpan? FirstCheckIn { get; set; }
    public TimeSpan? LastCheckOut { get; set; }
    public TimeSpan? ScheduledStart { get; set; }
    public TimeSpan? ScheduledEnd { get; set; }

    public decimal? TotalWorkHours { get; set; }
    public decimal? RequiredWorkHours { get; set; }
    public decimal? OvertimeHours { get; set; }
    public decimal? ShortageHours { get; set; }
    public int? LateMinutes { get; set; }
    public int? EarlyLeaveMinutes { get; set; }

    public DailyAttendanceStatus Status { get; set; }
    public bool IsLate { get; set; }
    public bool IsEarlyLeave { get; set; }
    public bool IsAbsent { get; set; }
    public bool IsOvertime { get; set; }
    public bool IsOnLeave { get; set; }
    public bool IsHoliday { get; set; }
    public bool IsWeekend { get; set; }
    public bool IsExcused { get; set; }

    public decimal? LateDeductionAmount { get; set; }
    public decimal? EarlyLeaveDeductionAmount { get; set; }
    public decimal? AbsenceDeductionAmount { get; set; }
    public decimal? TotalDeductionAmount { get; set; }
    public decimal? OvertimeAmount { get; set; }

    public string? Notes { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsProcessed { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrWorkShift? WorkShift { get; set; }
}
