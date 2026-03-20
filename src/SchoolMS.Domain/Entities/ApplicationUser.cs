using Microsoft.AspNetCore.Identity;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public int SchoolId { get; set; }
    public int? BranchId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public UserType UserType { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public virtual School School { get; set; } = null!;
    public virtual Branch? Branch { get; set; }
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}
