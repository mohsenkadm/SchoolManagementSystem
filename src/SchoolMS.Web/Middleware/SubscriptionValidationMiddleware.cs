using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Web.Middleware;

public class SubscriptionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionValidationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, SchoolDbContext dbContext, SignInManager<ApplicationUser> signInManager)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        // SuperAdmin bypasses all checks
        if (context.User.IsInRole("SuperAdmin"))
        {
            await _next(context);
            return;
        }

        // Allow logout, access denied, and login paths
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/account/logout") || path.Contains("/account/login") || path.Contains("/account/accessdenied") || path.Contains("/subscription-expired"))
        {
            await _next(context);
            return;
        }

        var schoolIdClaim = context.User.FindFirst("SchoolId")?.Value;
        if (schoolIdClaim == null || !int.TryParse(schoolIdClaim, out var schoolId))
        {
            // Non-SuperAdmin without SchoolId claim — stale session.
            // Sign out and redirect to login to get proper claims.
            await signInManager.SignOutAsync();
            context.Response.Redirect("/Account/Login");
            return;
        }

        // Check school is active
        var school = await dbContext.Schools.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == schoolId && !s.IsDeleted);

        if (school == null || !school.IsActive)
        {
            context.Response.Redirect("/subscription-expired?reason=inactive");
            return;
        }

        // Check active subscription
        var activeSub = await dbContext.SchoolSubscriptions.IgnoreQueryFilters()
            .Include(s => s.SystemSubscriptionPlan)
            .Where(s => s.SchoolId == schoolId && s.IsActive && !s.IsDeleted)
            .OrderByDescending(s => s.ActivatedAt)
            .FirstOrDefaultAsync();

        if (activeSub != null)
        {
            // Check expiry
            if (activeSub.ExpiryDate < DateTime.UtcNow)
            {
                context.Response.Redirect("/subscription-expired?reason=expired");
                return;
            }

            // Store plan limits in HttpContext.Items for later validation
            if (activeSub.SystemSubscriptionPlan != null)
            {
                context.Items["MaxUsers"] = activeSub.SystemSubscriptionPlan.MaxUsers;
                context.Items["MaxStudents"] = activeSub.SystemSubscriptionPlan.MaxStudents;
                context.Items["SubscriptionExpiry"] = activeSub.ExpiryDate;
                context.Items["PlanName"] = activeSub.SystemSubscriptionPlan.PlanName;
                context.Items["IncludesHrModule"] = activeSub.SystemSubscriptionPlan.IncludesHrModule;
                context.Items["IncludesCourses"] = activeSub.SystemSubscriptionPlan.IncludesCourses;
            }
        }

        await _next(context);
    }
}
