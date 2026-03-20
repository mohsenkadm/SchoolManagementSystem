using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? TitleAr { get; set; }
    public string? Content { get; set; }
    public string? ContentAr { get; set; }
    public AnnouncementPriority Priority { get; set; }
    public AnnouncementTarget Target { get; set; }
    public int? BranchId { get; set; }
    public int? GradeId { get; set; }
    public string? AttachmentUrl { get; set; }
    public bool IsPinned { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool SendNotification { get; set; }
    public int ViewCount { get; set; }

    public virtual Branch? Branch { get; set; }
    public virtual Grade? Grade { get; set; }
}
