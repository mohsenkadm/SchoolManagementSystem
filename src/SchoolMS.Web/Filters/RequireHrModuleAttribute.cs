using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireHrModuleAttribute : TypeFilterAttribute
{
    public RequireHrModuleAttribute() : base(typeof(RequireHrModuleFilter)) { }
}

public class RequireHrModuleFilter : IAsyncAuthorizationFilter
{
    private readonly SchoolDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public RequireHrModuleFilter(SchoolDbContext context, ITenantProvider tenantProvider)
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

        // Check if the school's active subscription plan includes HR
        var activeSub = await _context.SchoolSubscriptions
            .IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId.Value && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync();

        if (activeSub?.SystemSubscriptionPlan == null || !activeSub.SystemSubscriptionPlan.IncludesHrModule)
        {
            context.Result = new RedirectResult("/AccessDenied?reason=hr-module-not-in-plan");
        }
    }
}
