namespace SchoolMS.Domain.Entities;

public class FeeInstallment : BaseEntity
{
    public int StudentId { get; set; }
    public decimal TotalAmount { get; set; }
    public int NumberOfPayments { get; set; }
    public int? DefaultPaymentPlanId { get; set; }
    public int AcademicYearId { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual DefaultPaymentPlan? DefaultPaymentPlan { get; set; }
    public virtual AcademicYear AcademicYear { get; set; } = null!;
    public virtual ICollection<InstallmentPayment> InstallmentPayments { get; set; } = new List<InstallmentPayment>();
}
