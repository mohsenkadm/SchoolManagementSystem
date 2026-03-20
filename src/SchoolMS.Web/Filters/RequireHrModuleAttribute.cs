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
        var schoolId = _tenantProvider.GetCurrentSchoolId();
        if (!schoolId.HasValue)
        {
            context.Result = new RedirectResult("/AccessDenied?reason=no-school");
            return;
        }

        var school = await _context.Schools
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == schoolId.Value && !s.IsDeleted);

        if (school == null || !school.IsHrModuleEnabled)
        {
            context.Result = new RedirectResult("/AccessDenied?reason=hr-module-disabled");
        }
    }
}
