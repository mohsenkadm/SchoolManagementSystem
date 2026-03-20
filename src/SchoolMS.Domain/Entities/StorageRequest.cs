namespace SchoolMS.Domain.Entities;

public class StorageRequest : BaseEntity
{
    public int SchoolSubscriptionId { get; set; }
    public decimal RequestedGB { get; set; }
    public decimal PricePerGB { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
    public bool IsApproved { get; set; }
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }

    public virtual SchoolSubscription SchoolSubscription { get; set; } = null!;
}
