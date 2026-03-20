using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Web.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string page, string action)
        : base(typeof(HasPermissionFilter))
    {
        Arguments = new object[] { page, action };
    }
}

public class HasPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _page;
    private readonly string _action;
    private readonly SchoolDbContext _context;

    public HasPermissionFilter(string page, string action, SchoolDbContext context)
    {
        _page = page;
        _action = action;
        _context = context;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (user.IsInRole("SuperAdmin"))
            return;

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var hasPermission = await _context.UserPermissions
            .IgnoreQueryFilters()
            .Include(up => up.Permission)
            .AnyAsync(up =>
                up.UserId == userId &&
                up.IsGranted &&
                up.Permission.PageName == _page &&
                up.Permission.Action == _action &&
                !up.IsDeleted);

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
