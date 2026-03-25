using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireOnlinePlatform]
public class StudentSubscriptionsController : Controller
{
    private readonly IStudentSubscriptionService _service;
    private readonly IOnlineSubscriptionPlanService _planService;
    private readonly IStudentService _studentService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public StudentSubscriptionsController(
        IStudentSubscriptionService service,
        IOnlineSubscriptionPlanService planService,
        IStudentService studentService,
        IPlatformService platformService,
        IBranchService branchService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _planService = planService;
        _studentService = studentService;
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

    [HasPermission("StudentSubscriptions", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Student Subscriptions";
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
                : new List<StudentSubscriptionDto>();
        return View(all);
    }

    [HasPermission("StudentSubscriptions", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Student Subscription";
        await LoadCreateViewBags();
        return View();
    }

    [HttpPost, HasPermission("StudentSubscriptions", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentSubscriptionDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("Subscription Created", "A new subscription has been created for you", dto.StudentId, "Student", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("StudentSubscriptions", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Student Subscription";
        await LoadCreateViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("StudentSubscriptions", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentSubscriptionDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("StudentSubscriptions", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost, HasPermission("StudentSubscriptions", "Edit")]
    public async Task<IActionResult> Approve(int id) { await _service.UpdateStatusAsync(id, SubscriptionStatus.Approved); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("Subscription Approved", "A student subscription has been approved", new[] { "Student", "Parent" }, CurrentSchoolId.Value); return Ok(); }

    [HttpPost, HasPermission("StudentSubscriptions", "Edit")]
    public async Task<IActionResult> Reject(int id) { await _service.UpdateStatusAsync(id, SubscriptionStatus.Rejected); return Ok(); }

    [HttpPost, HasPermission("StudentSubscriptions", "Edit")]
    public async Task<IActionResult> SetPending(int id) { await _service.UpdateStatusAsync(id, SubscriptionStatus.Pending); return Ok(); }

    [HttpPost, HasPermission("StudentSubscriptions", "Edit")]
    public async Task<IActionResult> MarkAsPaid(int id) { await _service.MarkAsPaidAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentSubscriptions.xlsx");
    }

    private async Task LoadCreateViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Plans = IsSuperAdmin
            ? await _planService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _planService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<OnlineSubscriptionPlanDto>();
        ViewBag.Students = await _studentService.GetAllAsync();
        ViewBag.Statuses = Enum.GetValues<SubscriptionStatus>();
    }
}
