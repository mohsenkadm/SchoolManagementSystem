using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrDisciplinaryAction : BaseEntity
{
    public int EmployeeId { get; set; }
    public int? ViolationTypeId { get; set; }
    public DateTime IncidentDate { get; set; }
    public string? IncidentDescription { get; set; }
    public DisciplinaryActionType ActionType { get; set; }
    public int? WarningLevel { get; set; }
    public int? SuspensionDays { get; set; }
    public DateTime? SuspensionStartDate { get; set; }
    public DateTime? SuspensionEndDate { get; set; }
    public bool IsPaidSuspension { get; set; }
    public decimal? PenaltyAmount { get; set; }
    public string? EmployeeResponse { get; set; }
    public string? Witnesses { get; set; }
    public string? Evidence { get; set; }
    public string? DecisionNumber { get; set; }
    public DisciplinaryStatus Status { get; set; }
    public string? IssuedBy { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? Notes { get; set; }
    public bool NotifyEmployee { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrViolationType? ViolationType { get; set; }
}
