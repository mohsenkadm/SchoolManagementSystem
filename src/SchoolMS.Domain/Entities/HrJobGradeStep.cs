namespace SchoolMS.Domain.Entities;

public class HrJobGradeStep : BaseEntity
{
    public int JobGradeId { get; set; }
    public int StepNumber { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal AnnualIncrement { get; set; }
    public int YearsInStep { get; set; }

    public virtual HrJobGrade JobGrade { get; set; } = null!;
}
