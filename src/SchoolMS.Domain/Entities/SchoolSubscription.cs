namespace SchoolMS.Domain.Entities;

public class SchoolSubscription : BaseEntity
{
    public int SystemSubscriptionPlanId { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public decimal ExtraStorageGB { get; set; }
    public decimal ExtraStoragePrice { get; set; }

    public virtual School School { get; set; } = null!;
    public virtual SystemSubscriptionPlan SystemSubscriptionPlan { get; set; } = null!;
    public virtual ICollection<StorageRequest> StorageRequests { get; set; } = new List<StorageRequest>();
}
