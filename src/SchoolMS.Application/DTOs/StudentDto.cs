namespace SchoolMS.Application.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ParentPhone { get; set; }
    public string? ParentName { get; set; }
    public string? Email { get; set; }
    public string? ProfileImage { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int ClassRoomId { get; set; }
    public string? GradeName { get; set; }
    public string? DivisionName { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public string? Notes { get; set; }
}

public class CreateStudentDto
{
    public string FullName { get; set; } = string.Empty;
    public string? FullNameAr { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? ParentPhone { get; set; }
    public string? ParentName { get; set; }
    public string? Email { get; set; }
    public string? BadgeCardNumber { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int BranchId { get; set; }
    public int ClassRoomId { get; set; }
    public int AcademicYearId { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStudentDto : CreateStudentDto
{
    public int Id { get; set; }
}
