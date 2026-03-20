using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrSalaryDetail : BaseEntity
{
    public int EmployeeId { get; set; }
    public decimal BaseSalary { get; set; }
    public string? Currency { get; set; }
    public SalaryType SalaryType { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual ICollection<HrSalaryAllowance> Allowances { get; set; } = new List<HrSalaryAllowance>();
    public virtual ICollection<HrSalaryDeduction> Deductions { get; set; } = new List<HrSalaryDeduction>();
}
