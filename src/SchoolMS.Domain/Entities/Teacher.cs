namespace SchoolMS.Domain.Entities;

public class Teacher : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public decimal BaseSalary { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<TeacherAssignment> TeacherAssignments { get; set; } = new List<TeacherAssignment>();
    public virtual ICollection<Homework> Homeworks { get; set; } = new List<Homework>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
