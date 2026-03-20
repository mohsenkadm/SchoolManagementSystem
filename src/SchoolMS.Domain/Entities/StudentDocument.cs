namespace SchoolMS.Domain.Entities;

public class StudentDocument : BaseEntity
{
    public int StudentId { get; set; }
    public string DocumentName { get; set; } = string.Empty;
    public string? DocumentType { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    public string? Notes { get; set; }

    public virtual Student Student { get; set; } = null!;
}
