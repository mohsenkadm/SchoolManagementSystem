namespace SchoolMS.Domain.Entities;

public class HrEmployeeDocument : BaseEntity
{
    public int EmployeeId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public string? Notes { get; set; }

    public virtual HrEmployee Employee { get; set; } = null!;
}
