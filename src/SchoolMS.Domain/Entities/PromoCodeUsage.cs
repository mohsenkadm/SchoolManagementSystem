namespace SchoolMS.Domain.Entities;

public class PromoCodeUsage : BaseEntity
{
    public int PromoCodeId { get; set; }
    public int StudentId { get; set; }
    public int? StudentSubscriptionId { get; set; }
    public DateTime UsedAt { get; set; }

    public virtual PromoCode PromoCode { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual StudentSubscription? StudentSubscription { get; set; }
}
