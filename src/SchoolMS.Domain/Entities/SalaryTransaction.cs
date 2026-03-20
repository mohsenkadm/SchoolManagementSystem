using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class SalaryTransaction : BaseEntity
{
    public int MonthlySalaryId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Notes { get; set; }

    public virtual MonthlySalary MonthlySalary { get; set; } = null!;
}
