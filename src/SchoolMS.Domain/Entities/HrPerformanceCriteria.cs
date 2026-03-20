namespace SchoolMS.Domain.Entities;

public class HrPerformanceCriteria : BaseEntity
{
    public string CriteriaName { get; set; } = string.Empty;
    public string? CriteriaNameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public decimal Weight { get; set; }
    public decimal MaxScore { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}
