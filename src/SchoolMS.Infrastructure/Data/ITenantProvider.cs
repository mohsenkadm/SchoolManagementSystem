namespace SchoolMS.Infrastructure.Data;

public interface ITenantProvider
{
    int? GetCurrentSchoolId();
    int? GetCurrentBranchId();
    string? GetCurrentUserId();
}
