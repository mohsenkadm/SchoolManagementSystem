using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Infrastructure.Data;
using SchoolMS.StudentPortal.Filters;

namespace SchoolMS.StudentPortal.Controllers;

[StudentAuth]
public class SubscriptionsController : Controller
{
    private readonly SchoolDbContext _context;

    public SubscriptionsController(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var subscriptions = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .Include(s => s.OnlineSubscriptionPlan).ThenInclude(p => p.Subject)
            .Where(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(subscriptions);
    }

    public async Task<IActionResult> Plans()
    {
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;

        var plans = await _context.Set<OnlineSubscriptionPlan>().IgnoreQueryFilters()
            .Include(p => p.Subject)
            .Where(p => p.SchoolId == schoolId && !p.IsDeleted)
            .ToListAsync();

        var subscribedPlanIds = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .Where(s => s.StudentId == studentId && s.SchoolId == schoolId && !s.IsDeleted
                && (s.Status == SubscriptionStatus.Pending || (s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow)))
            .Select(s => s.OnlineSubscriptionPlanId)
            .ToListAsync();

        ViewBag.SubscribedPlanIds = subscribedPlanIds.ToHashSet();
        return View(plans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(int planId)
    {
        var studentId = HttpContext.Session.GetInt32("StudentId")!.Value;
        var schoolId = HttpContext.Session.GetInt32("SchoolId")!.Value;

        var plan = await _context.Set<OnlineSubscriptionPlan>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == planId && p.SchoolId == schoolId && !p.IsDeleted);

        if (plan == null)
            return RedirectToAction("Plans");

        // Check if already has an active or pending subscription for this plan
        var existing = await _context.Set<StudentSubscription>().IgnoreQueryFilters()
            .AnyAsync(s => s.StudentId == studentId && s.OnlineSubscriptionPlanId == planId
                && s.SchoolId == schoolId && !s.IsDeleted
                && (s.Status == SubscriptionStatus.Pending || (s.Status == SubscriptionStatus.Approved && s.EndDate >= DateTime.UtcNow)));

        if (existing)
            return RedirectToAction("Index");

        var subscription = new StudentSubscription
        {
            StudentId = studentId,
            OnlineSubscriptionPlanId = plan.Id,
            SchoolId = schoolId,
            Status = SubscriptionStatus.Pending,
            OriginalAmount = plan.Price,
            DiscountAmount = 0,
            PaidAmount = plan.Price,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(plan.DurationMonths),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "StudentPortal"
        };

        _context.Set<StudentSubscription>().Add(subscription);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
