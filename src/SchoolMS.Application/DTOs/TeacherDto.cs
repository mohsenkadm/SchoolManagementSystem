namespace SchoolMS.Application.DTOs;

public class TeacherDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public decimal BaseSalary { get; set; }
}

public class CreateTeacherDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public decimal BaseSalary { get; set; }
}

public class UpdateTeacherDto : CreateTeacherDto
{
    public int Id { get; set; }
}
