using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class TeacherEarning : BaseEntity
{
    public int TeacherId { get; set; }
    public int? CourseId { get; set; }
    public int? StudentSubscriptionId { get; set; }
    public decimal SubscriptionAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal EarningAmount { get; set; }
    public TeacherEarningStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? PaidAt { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
    public virtual Course? Course { get; set; }
    public virtual StudentSubscription? StudentSubscription { get; set; }
}
