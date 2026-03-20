namespace SchoolMS.Domain.Entities;

public class Permission : BaseEntity
{
    public string PageName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
