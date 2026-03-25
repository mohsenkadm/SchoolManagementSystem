using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireOnlinePlatformAttribute : TypeFilterAttribute
{
    public RequireOnlinePlatformAttribute() : base(typeof(RequireOnlinePlatformFilter)) { }
}

public class RequireOnlinePlatformFilter : IAsyncAuthorizationFilter
{
    private readonly SchoolDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public RequireOnlinePlatformFilter(SchoolDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _tenantProvider = tenantProvider;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // SuperAdmin bypasses all checks
        if (context.HttpContext.User.IsInRole("SuperAdmin"))
            return;

        var schoolId = _tenantProvider.GetCurrentSchoolId();
        if (!schoolId.HasValue)
        {
            context.Result = new RedirectResult("/AccessDenied?reason=no-school");
            return;
        }

        // Check if the school's active subscription plan includes Courses (online platform)
        var activeSub = await _context.SchoolSubscriptions
            .IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId.Value && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync();

        if (activeSub?.SystemSubscriptionPlan == null || !activeSub.SystemSubscriptionPlan.IncludesCourses)
        {
            context.Result = new RedirectResult("/AccessDenied?reason=online-platform-not-in-plan");
        }
    }
}
