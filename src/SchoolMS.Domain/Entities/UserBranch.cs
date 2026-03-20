namespace SchoolMS.Domain.Entities;

public class UserBranch : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int BranchId { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
