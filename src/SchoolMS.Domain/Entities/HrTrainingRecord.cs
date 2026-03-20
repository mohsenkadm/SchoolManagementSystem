using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrTrainingRecord : BaseEntity
{
    public int EmployeeId { get; set; }
    public int TrainingProgramId { get; set; }
    public TrainingParticipantStatus Status { get; set; }
    public decimal? Score { get; set; }
    public decimal? AttendancePercentage { get; set; }
    public bool CertificateIssued { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Feedback { get; set; }
    public int? Rating { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
    public virtual HrTrainingProgram TrainingProgram { get; set; } = null!;
}
