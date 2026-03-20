namespace SchoolMS.Domain.Entities;

public class ExpenseType : BaseEntity
{
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
