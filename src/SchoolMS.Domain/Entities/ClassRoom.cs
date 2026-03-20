namespace SchoolMS.Domain.Entities;

public class ClassRoom : BaseEntity
{
    public int GradeId { get; set; }
    public int DivisionId { get; set; }
    public int AcademicYearId { get; set; }
    public int BranchId { get; set; }

    public virtual Grade Grade { get; set; } = null!;
    public virtual Division Division { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    public virtual ICollection<WeeklySchedule> WeeklySchedules { get; set; } = new List<WeeklySchedule>();
    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
}
