namespace SchoolMS.Domain.Entities;

public class ExamType : BaseEntity
{
    public string TypeName { get; set; } = string.Empty;

    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();
    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
