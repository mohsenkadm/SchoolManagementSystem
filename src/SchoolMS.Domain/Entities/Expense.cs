namespace SchoolMS.Domain.Entities;

public class Expense : BaseEntity
{
    public int ExpenseTypeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int BranchId { get; set; }

    public virtual ExpenseType ExpenseType { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
