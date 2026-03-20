using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

// ===== Online Subscription Plan =====
public class OnlineSubscriptionPlanDto
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationMonths { get; set; }
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public int StudentSubscriptionCount { get; set; }
}

// ===== Student Subscription =====
public class StudentSubscriptionDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int OnlineSubscriptionPlanId { get; set; }
    public string? PlanName { get; set; }
    public string? SubjectName { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public SubscriptionStatus Status { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? PromoCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public string? BranchName { get; set; }
}

// ===== Promo Code =====
public class PromoCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int MaxUsage { get; set; }
    public int CurrentUsage { get; set; }
    public bool IsUnlimited { get; set; }
    public bool IsActive { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public List<PromoCodeUsageDto> Usages { get; set; } = new();
}

public class PromoCodeUsageDto
{
    public int Id { get; set; }
    public int PromoCodeId { get; set; }
    public string? PromoCodeText { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; }
    public int? StudentSubscriptionId { get; set; }
    public DateTime UsedAt { get; set; }
}

// ===== API Subscribe Request =====
public class SubscribeRequestDto
{
    public int StudentId { get; set; }
    public int OnlineSubscriptionPlanId { get; set; }
    public string? PromoCode { get; set; }
    public int SchoolId { get; set; }
}
