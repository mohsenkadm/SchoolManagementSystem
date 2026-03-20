using Microsoft.AspNetCore.Mvc;

namespace SchoolMS.Web.Controllers;

public class SubscriptionController : Controller
{
    [HttpGet("/subscription-expired")]
    public IActionResult Expired(string? reason)
    {
        ViewData["Title"] = "Subscription Expired";
        ViewData["Reason"] = reason;
        return View("Expired");
    }
}
