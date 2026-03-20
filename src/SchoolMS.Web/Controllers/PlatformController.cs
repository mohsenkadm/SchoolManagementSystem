using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class PlatformController : Controller
{
    private readonly IPlatformService _service;
    private readonly IStorageQuotaService _storageQuotaService;
    private readonly IOneSignalNotificationService _pushService;

    public PlatformController(IPlatformService service, IStorageQuotaService storageQuotaService, IOneSignalNotificationService pushService)
    {
        _service = service;
        _storageQuotaService = storageQuotaService;
        _pushService = pushService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Platform Management";
        var dashboard = await _service.GetDashboardAsync();
        return View(dashboard);
    }

    // ===== Schools =====
    public async Task<IActionResult> Schools()
    {
        ViewData["Title"] = "Manage Schools";
        ViewBag.Plans = await _service.GetAllPlansAsync();
        var schools = await _service.GetAllSchoolsAsync();
        return View(schools);
    }

    [HttpGet]
    public async Task<IActionResult> GetSchool(int id) => Json(await _service.GetSchoolByIdAsync(id));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSchool([FromBody] SchoolCreateDto dto)
    {
        try { var result = await _service.CreateSchoolAsync(dto); return Ok(result); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSchool([FromBody] SchoolUpdateDto dto)
        => Ok(await _service.UpdateSchoolAsync(dto));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSchool(int id)
    { await _service.ToggleSchoolActiveAsync(id); return Ok(); }

    [HttpDelete("{id}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSchool(int id)
    { await _service.DeleteSchoolAsync(id); return Ok(); }

    // ===== Subscription Plans =====
    public async Task<IActionResult> Plans()
    {
        ViewData["Title"] = "Subscription Plans";
        var plans = await _service.GetAllPlansAsync();
        return View(plans);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePlan([FromBody] SubscriptionPlanDto dto)
        => Ok(await _service.CreatePlanAsync(dto));

    [HttpPut, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePlan([FromBody] SubscriptionPlanDto dto)
        => Ok(await _service.UpdatePlanAsync(dto));

    [HttpDelete("Plan/{id}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePlan(int id)
    { await _service.DeletePlanAsync(id); return Ok(); }

    // ===== School Subscriptions =====
    public async Task<IActionResult> Subscriptions()
    {
        ViewData["Title"] = "School Subscriptions";
        ViewBag.Plans = await _service.GetAllPlansAsync();
        ViewBag.Schools = await _service.GetAllSchoolsAsync();
        ViewBag.StorageRequests = await _service.GetPendingStorageRequestsAsync();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetSubscriptions(int schoolId)
        => Json(await _service.GetSchoolSubscriptionsAsync(schoolId));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignSubscription([FromBody] SchoolSubscriptionDto dto)
    {
        var result = await _service.AssignSubscriptionAsync(dto);
        await _pushService.SendToSchoolAsync("Subscription Assigned", "A new subscription plan has been assigned to your school", dto.SchoolId);
        return Ok(result);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelSubscription(int id)
    { await _service.CancelSubscriptionAsync(id); return Ok(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RenewSubscription(int id)
    { await _service.RenewSubscriptionAsync(id); return Ok(); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExtraStorage([FromBody] AddExtraStorageRequest req)
    {
        await _service.AddExtraStorageAsync(req.SubscriptionId, req.ExtraGB, req.PricePerGB);
        return Ok();
    }

    // ===== Storage Requests =====
    [HttpGet]
    public async Task<IActionResult> GetStorageRequests()
        => Json(await _service.GetPendingStorageRequestsAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveStorageRequest(int id)
    {
        await _service.ApproveStorageRequestAsync(id, User.Identity?.Name ?? "SuperAdmin");
        return Ok();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectStorageRequest(int id)
    {
        await _service.RejectStorageRequestAsync(id, User.Identity?.Name ?? "SuperAdmin");
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetSchoolStorageQuota(int schoolId)
    {
        var quota = await _storageQuotaService.GetQuotaAsync(schoolId);
        return quota == null ? NotFound() : Json(quota);
    }
}
