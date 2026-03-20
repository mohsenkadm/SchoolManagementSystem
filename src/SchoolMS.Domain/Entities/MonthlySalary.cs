namespace SchoolMS.Domain.Entities;

public class MonthlySalary : BaseEntity
{
    public int SalarySetupId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }

    public virtual SalarySetup SalarySetup { get; set; } = null!;
    public virtual ICollection<SalaryTransaction> SalaryTransactions { get; set; } = new List<SalaryTransaction>();
}
