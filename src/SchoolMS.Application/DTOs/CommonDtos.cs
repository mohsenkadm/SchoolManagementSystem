namespace SchoolMS.Application.DTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class DivisionDto
{
    public int Id { get; set; }
    public string DivisionName { get; set; } = string.Empty;
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class GradeDto
{
    public int Id { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public int DivisionId { get; set; }
    public string? DivisionName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class ClassRoomDto
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public string? GradeName { get; set; }
    public int DivisionId { get; set; }
    public string? DivisionName { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
}

public class AcademicYearDto
{
    public int Id { get; set; }
    public string YearName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class BranchDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public bool IsActive { get; set; }
}

public class ExamTypeDto
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class ExpenseTypeDto
{
    public int Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
}

public class StaffDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public int BranchId { get; set; }
    public decimal BaseSalary { get; set; }
    public int SchoolId { get; set; }
}
