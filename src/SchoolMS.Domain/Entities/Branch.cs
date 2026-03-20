namespace SchoolMS.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
    public virtual ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    public virtual ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    public virtual ICollection<ClassRoom> ClassRooms { get; set; } = new List<ClassRoom>();
}
