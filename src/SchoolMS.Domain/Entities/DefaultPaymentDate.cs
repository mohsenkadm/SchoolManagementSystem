namespace SchoolMS.Domain.Entities;

public class DefaultPaymentDate : BaseEntity
{
    public int DefaultPaymentPlanId { get; set; }
    public int PaymentNumber { get; set; }
    public DateTime DueDate { get; set; }

    public virtual DefaultPaymentPlan DefaultPaymentPlan { get; set; } = null!;
}
