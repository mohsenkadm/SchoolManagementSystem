namespace SchoolMS.Domain.Entities;

public class ExamSchedule : BaseEntity
{
    public int ExamTypeId { get; set; }
    public int ClassRoomId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public DateTime ExamDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int AcademicYearId { get; set; }

    public virtual ExamType ExamType { get; set; } = null!;
    public virtual ClassRoom ClassRoom { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teacher Teacher { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
}
