using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrPerformanceCycle : BaseEntity
{
    public string CycleName { get; set; } = string.Empty;
    public string? CycleNameAr { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CycleStatus Status { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<HrPerformanceReview> Reviews { get; set; } = new List<HrPerformanceReview>();
}
