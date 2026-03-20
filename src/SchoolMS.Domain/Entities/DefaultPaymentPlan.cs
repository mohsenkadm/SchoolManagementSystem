namespace SchoolMS.Domain.Entities;

public class DefaultPaymentPlan : BaseEntity
{
    public string PlanName { get; set; } = string.Empty;
    public int NumberOfPayments { get; set; }

    public virtual ICollection<DefaultPaymentDate> PaymentDates { get; set; } = new List<DefaultPaymentDate>();
    public virtual ICollection<FeeInstallment> FeeInstallments { get; set; } = new List<FeeInstallment>();
}
