using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolMS.StudentPortal.Filters;

public class StudentAuthAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var studentId = session.GetInt32("StudentId");

        if (studentId == null && context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            // Restore session from persistent cookie claims
            var idClaim = context.HttpContext.User.FindFirst("StudentId")?.Value;
            var schoolIdClaim = context.HttpContext.User.FindFirst("SchoolId")?.Value;

            if (int.TryParse(idClaim, out var sid) && int.TryParse(schoolIdClaim, out var schId))
            {
                session.SetInt32("StudentId", sid);
                session.SetInt32("SchoolId", schId);
                session.SetString("StudentName",
                    context.HttpContext.User.FindFirst("StudentName")?.Value ?? "");
                session.SetString("SchoolSlug",
                    context.HttpContext.User.FindFirst("SchoolSlug")?.Value ?? "");
                session.SetString("DeviceId",
                    context.HttpContext.User.FindFirst("DeviceId")?.Value ?? "");
                var branchClaim = context.HttpContext.User.FindFirst("BranchId")?.Value;
                if (int.TryParse(branchClaim, out var bid))
                    session.SetInt32("BranchId", bid);

                studentId = sid;
            }
        }

        if (studentId == null)
        {
            var slug = session.GetString("SchoolSlug")
                ?? context.HttpContext.User.FindFirst("SchoolSlug")?.Value;
            context.Result = new RedirectToActionResult("Login", "Account", new { slug });
            return;
        }

        base.OnActionExecuting(context);
    }
}
