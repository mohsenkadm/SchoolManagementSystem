namespace SchoolMS.Domain.Entities;

public class AcademicYear : BaseEntity
{
    public string YearName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }

    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
}
