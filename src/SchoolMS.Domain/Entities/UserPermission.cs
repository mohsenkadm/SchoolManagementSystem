namespace SchoolMS.Domain.Entities;

public class UserPermission : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int PermissionId { get; set; }
    public bool IsGranted { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
