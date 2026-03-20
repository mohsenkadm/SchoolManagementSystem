namespace SchoolMS.Domain.Entities;

public class WeeklySchedule : BaseEntity
{
    public int ClassRoomId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int AcademicYearId { get; set; }

    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
}
