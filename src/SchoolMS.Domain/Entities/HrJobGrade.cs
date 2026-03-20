namespace SchoolMS.Domain.Entities;

public class HrJobGrade : BaseEntity
{
    public string GradeName { get; set; } = string.Empty;
    public string? GradeNameAr { get; set; }
    public int GradeLevel { get; set; }
    public string? Description { get; set; }
    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public decimal DefaultAllowancePercentage { get; set; }
    public int? MinYearsExperience { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<HrJobGradeStep> Steps { get; set; } = new List<HrJobGradeStep>();
}
