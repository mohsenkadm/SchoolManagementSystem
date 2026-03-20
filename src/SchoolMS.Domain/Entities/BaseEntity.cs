namespace SchoolMS.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public virtual School School { get; set; } = null!;
}
