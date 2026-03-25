using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize(Roles = "SuperAdmin,SchoolAdmin")]
public class SchoolSubscriptionsController : Controller
{
    private readonly IPlatformService _platformService;

    public SchoolSubscriptionsController(IPlatformService platformService)
    {
        _platformService = platformService;
    }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    /// <summary>
    /// SuperAdmin: view all schools with subscriptions.
    /// SchoolAdmin: view own school subscription history.
    /// </summary>
    [HasPermission("SchoolSubscriptions", "View")]
    public async Task<IActionResult> Index()
    {
        if (IsSuperAdmin)
        {
            ViewData["Title"] = "All School Subscriptions";
            var schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Plans = await _platformService.GetAllPlansAsync();
            return View("SuperAdminIndex", schools);
        }

        // SchoolAdmin → redirect to own school page
        if (!CurrentSchoolId.HasValue)
            return RedirectToAction("Index", "Home");

        return RedirectToAction(nameof(MySubscription));
    }

    /// <summary>
    /// SchoolAdmin: view own school subscription history + active plan.
    /// </summary>
    [HasPermission("SchoolSubscriptions", "View")]
    public async Task<IActionResult> MySubscription()
    {
        if (!CurrentSchoolId.HasValue)
            return RedirectToAction("Index", "Home");

        ViewData["Title"] = "My Subscription";
        var subscriptions = await _platformService.GetSchoolSubscriptionsAsync(CurrentSchoolId.Value);
        return View(subscriptions);
    }

    /// <summary>
    /// SuperAdmin: view a specific school's subscription history.
    /// </summary>
    [Authorize(Roles = "SuperAdmin")]
    [HasPermission("SchoolSubscriptions", "View")]
    public async Task<IActionResult> SchoolDetail(int schoolId)
    {
        ViewData["Title"] = "School Subscription History";
        var school = await _platformService.GetSchoolByIdAsync(schoolId);
        if (school == null) return NotFound();

        ViewBag.School = school;
        var subscriptions = await _platformService.GetSchoolSubscriptionsAsync(schoolId);
        return View("MySubscription", subscriptions);
    }

    /// <summary>
    /// AJAX: Get subscriptions for a specific school.
    /// </summary>
    [HttpGet]
    [HasPermission("SchoolSubscriptions", "View")]
    public async Task<IActionResult> GetSubscriptions(int schoolId)
    {
        if (!IsSuperAdmin && CurrentSchoolId != schoolId)
            return Forbid();

        return Json(await _platformService.GetSchoolSubscriptionsAsync(schoolId));
    }
}
