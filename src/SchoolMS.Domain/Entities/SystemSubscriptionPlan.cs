namespace SchoolMS.Domain.Entities;

public class SystemSubscriptionPlan : BaseEntity
{
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxUsers { get; set; }
    public int MaxStudents { get; set; }
    public int DurationMonths { get; set; }
    public bool IncludesHrModule { get; set; }
    public bool IncludesCourses { get; set; }
    public bool IncludesLiveStream { get; set; }
    public decimal StorageLimitGB { get; set; }

    public virtual ICollection<SchoolSubscription> SchoolSubscriptions { get; set; } = new List<SchoolSubscription>();
}
