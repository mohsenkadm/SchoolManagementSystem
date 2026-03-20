namespace SchoolMS.Domain.Entities;

public class Staff : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public decimal BaseSalary { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
