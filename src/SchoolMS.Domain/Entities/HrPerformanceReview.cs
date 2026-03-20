using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrPerformanceReview : BaseEntity
{
    public int EmployeeId { get; set; }
    public int PerformanceCycleId { get; set; }
    public int ReviewerId { get; set; }

    public decimal TotalScore { get; set; }
    public decimal MaxPossibleScore { get; set; }
    public decimal Percentage { get; set; }
    public string? PerformanceRating { get; set; }

    public string? Strengths { get; set; }
    public string? AreasForImprovement { get; set; }
    public string? Goals { get; set; }
    public string? ManagerComments { get; set; }
    public string? EmployeeSelfAssessment { get; set; }
    public string? EmployeeComments { get; set; }

    public string? Recommendation { get; set; }
    public decimal? RecommendedSalaryIncrease { get; set; }

    public ReviewStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrPerformanceCycle PerformanceCycle { get; set; } = null!;
    public virtual ICollection<HrPerformanceScore> Scores { get; set; } = new List<HrPerformanceScore>();
}
