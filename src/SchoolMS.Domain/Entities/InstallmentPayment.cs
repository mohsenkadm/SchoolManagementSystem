using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class InstallmentPayment : BaseEntity
{
    public int FeeInstallmentId { get; set; }
    public int PaymentNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public PaymentStatus Status { get; set; }

    public virtual FeeInstallment FeeInstallment { get; set; } = null!;
}
