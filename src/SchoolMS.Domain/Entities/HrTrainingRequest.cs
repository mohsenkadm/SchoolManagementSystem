using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrTrainingRequest : BaseEntity
{
    public int EmployeeId { get; set; }
    public string RequestedTraining { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string? Category { get; set; }
    public decimal? EstimatedCost { get; set; }
    public DateTime? PreferredStartDate { get; set; }
    public TrainingRequestStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
    public int? AssignedTrainingProgramId { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrTrainingProgram? AssignedTrainingProgram { get; set; }
}
