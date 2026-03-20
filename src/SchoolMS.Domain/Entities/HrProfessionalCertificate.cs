namespace SchoolMS.Domain.Entities;

public class HrProfessionalCertificate : BaseEntity
{
    public int EmployeeId { get; set; }
    public string CertificateName { get; set; } = string.Empty;
    public string? IssuingBody { get; set; }
    public string? CertificateNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public string? AttachmentPath { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
