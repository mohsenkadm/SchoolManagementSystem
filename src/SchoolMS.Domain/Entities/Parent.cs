namespace SchoolMS.Domain.Entities;

public class Parent : BaseEntity
{
    public string FatherName { get; set; } = string.Empty;
    public string? FatherNameAr { get; set; }
    public string? FatherPhone { get; set; }
    public string? FatherEmail { get; set; }
    public string? FatherOccupation { get; set; }
    public string? MotherName { get; set; }
    public string? MotherNameAr { get; set; }
    public string? MotherPhone { get; set; }
    public string? MotherEmail { get; set; }
    public string? MotherOccupation { get; set; }
    public string? GuardianName { get; set; }
    public string? GuardianPhone { get; set; }
    public string? GuardianRelation { get; set; }
    public string? Address { get; set; }
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    public string? ProfileImage { get; set; }

    public virtual ICollection<Student> Children { get; set; } = new List<Student>();
}
