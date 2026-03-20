namespace SchoolMS.Domain.Entities;

public class StudentGrade : BaseEntity
{
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int ExamTypeId { get; set; }
    public decimal Mark { get; set; }
    public decimal MaxMark { get; set; }
    public string? GradeLetter { get; set; }
    public int AcademicYearId { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual ExamType ExamType { get; set; } = null!;
    public virtual AcademicYear AcademicYear { get; set; } = null!;
}
