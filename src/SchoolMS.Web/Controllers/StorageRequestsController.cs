using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class StorageRequestsController : Controller
{
    private readonly IStorageQuotaService _storageService;

    public StorageRequestsController(IStorageQuotaService storageService)
    {
        _storageService = storageService;
    }

    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    public async Task<IActionResult> Index()
    {
        var schoolId = CurrentSchoolId;
        if (!schoolId.HasValue) return Forbid();

        ViewData["Title"] = "Storage Requests";
        var quota = await _storageService.GetQuotaAsync(schoolId.Value);
        var requests = await _storageService.GetSchoolRequestsAsync(schoolId.Value);
        var plans = await _storageService.GetActiveStoragePlansAsync();
        ViewBag.Quota = quota;
        ViewBag.StoragePlans = plans;
        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> GetQuota()
    {
        var schoolId = CurrentSchoolId;
        if (!schoolId.HasValue) return Forbid();
        var quota = await _storageService.GetQuotaAsync(schoolId.Value);
        return quota == null ? NotFound() : Json(quota);
    }

    [HttpGet]
    public async Task<IActionResult> GetStoragePlans()
    {
        var plans = await _storageService.GetActiveStoragePlansAsync();
        return Json(plans);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] StorageRequestInput input)
    {
        var schoolId = CurrentSchoolId;
        if (!schoolId.HasValue) return Forbid();

        try
        {
            var result = await _storageService.RequestExtraStorageAsync(schoolId.Value, input.StoragePlanId, input.Notes);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public class StorageRequestInput
    {
        public int StoragePlanId { get; set; }
        public string? Notes { get; set; }
    }
}
