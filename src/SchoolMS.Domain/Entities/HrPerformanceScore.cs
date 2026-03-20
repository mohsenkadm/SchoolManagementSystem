namespace SchoolMS.Domain.Entities;

public class HrPerformanceScore : BaseEntity
{
    public int PerformanceReviewId { get; set; }
    public int PerformanceCriteriaId { get; set; }
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal WeightedScore { get; set; }
    public string? Comments { get; set; }

    public virtual HrPerformanceReview PerformanceReview { get; set; } = null!;
    public virtual HrPerformanceCriteria Criteria { get; set; } = null!;
}
