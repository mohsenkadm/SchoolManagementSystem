using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class HrTrainingProgram : BaseEntity
{
    public string ProgramName { get; set; } = string.Empty;
    public string? ProgramNameAr { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Provider { get; set; }
    public string? Trainer { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationHours { get; set; }
    public int MaxParticipants { get; set; }
    public decimal? Cost { get; set; }
    public decimal? CostPerParticipant { get; set; }
    public TrainingStatus Status { get; set; }
    public bool IsMandatory { get; set; }
    public string? AttachmentPath { get; set; }
    public string? CertificateTemplatePath { get; set; }
    public string? Notes { get; set; }

    public virtual ICollection<HrTrainingRecord> Participants { get; set; } = new List<HrTrainingRecord>();
}
