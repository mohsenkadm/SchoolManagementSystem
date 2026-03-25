namespace SchoolMS.Domain.Entities;

public class StoragePlan : BaseEntity
{
    public string PlanName { get; set; } = string.Empty;
    public decimal StorageGB { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<StorageRequest> StorageRequests { get; set; } = new List<StorageRequest>();
}
