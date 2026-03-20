using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class OnlineSubscriptionPlan : BaseEntity
{
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMonths { get; set; }
    public int SubjectId { get; set; }
    public SubscriptionType SubscriptionType { get; set; }

    public virtual Subject Subject { get; set; } = null!;
    public virtual ICollection<StudentSubscription> StudentSubscriptions { get; set; } = new List<StudentSubscription>();
}
