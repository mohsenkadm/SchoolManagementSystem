using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class SalarySetup : BaseEntity
{
    public int PersonId { get; set; }
    public PersonType PersonType { get; set; }
    public decimal BaseSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }

    public virtual ICollection<MonthlySalary> MonthlySalaries { get; set; } = new List<MonthlySalary>();
}
