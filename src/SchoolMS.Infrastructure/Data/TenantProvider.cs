using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SchoolMS.Infrastructure.Data;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? GetCurrentSchoolId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // أولاً: محاولة جلب schoolId من مسار الـ URL (مثل api/{schoolId}/students)
        if (httpContext.Request.RouteValues.TryGetValue("schoolId", out var routeValue)
            && routeValue != null
            && int.TryParse(routeValue.ToString(), out var routeId))
        {
            return routeId;
        }

        // ثانياً: الرجوع للـ Claims في التوكن
        var claim = httpContext.User?.FindFirst("SchoolId");
        return claim != null && int.TryParse(claim.Value, out var claimId) ? claimId : null;
    }

    public int? GetCurrentBranchId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("BranchId");
        return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
    }

    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
