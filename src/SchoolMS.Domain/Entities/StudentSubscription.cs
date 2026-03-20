using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class StudentSubscription : BaseEntity
{
    public int StudentId { get; set; }
    public int OnlineSubscriptionPlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PromoCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual OnlineSubscriptionPlan OnlineSubscriptionPlan { get; set; } = null!;
}
