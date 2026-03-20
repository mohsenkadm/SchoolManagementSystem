namespace SchoolMS.Domain.Entities;

public class Subject : BaseEntity
{
    public string SubjectName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
    public virtual ICollection<WeeklySchedule> WeeklySchedules { get; set; } = new List<WeeklySchedule>();
    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
}
