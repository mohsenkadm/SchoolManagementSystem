using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireOnlinePlatform]
public class OnlineSubscriptionPlansController : Controller
{
    private readonly IOnlineSubscriptionPlanService _service;
    private readonly ISubjectService _subjectService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public OnlineSubscriptionPlansController(
        IOnlineSubscriptionPlanService service,
        ISubjectService subjectService,
        IPlatformService platformService,
        IBranchService branchService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _subjectService = subjectService;
        _platformService = platformService;
        _branchService = branchService;
        _pushService = pushService;
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

    [HasPermission("OnlineSubscriptionPlans", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Online Subscription Plans";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<OnlineSubscriptionPlanDto>();
        return View(all);
    }

    [HasPermission("OnlineSubscriptionPlans", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Online Subscription Plan";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("OnlineSubscriptionPlans", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OnlineSubscriptionPlanDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Subscription Plan", $"{dto.PlanName} is now available", new[] { "Student", "Parent" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("OnlineSubscriptionPlans", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Online Subscription Plan";
        await LoadCreateViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("OnlineSubscriptionPlans", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(OnlineSubscriptionPlanDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Subscription Plan Updated", $"{dto.PlanName} has been updated", new[] { "Student", "Parent" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("OnlineSubscriptionPlans", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OnlineSubscriptionPlans.xlsx");
    }

    private async Task LoadCreateViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Subjects = IsSuperAdmin
            ? await _subjectService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _subjectService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
    }
}
