using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.DTOs;

public class TeacherEarningDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int? StudentSubscriptionId { get; set; }
    public string? StudentName { get; set; }
    public string? PlanName { get; set; }
    public decimal SubscriptionAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal EarningAmount { get; set; }
    public TeacherEarningStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime? PaidAt { get; set; }
    public int SchoolId { get; set; }
    public string? SchoolName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TeacherEarningSummaryDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public decimal TotalEarnings { get; set; }
    public decimal PendingEarnings { get; set; }
    public decimal ApprovedEarnings { get; set; }
    public decimal PaidEarnings { get; set; }
    public int TotalTransactions { get; set; }
}
