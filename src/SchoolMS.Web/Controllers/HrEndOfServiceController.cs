using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrEndOfServiceController : Controller
{
    private readonly IHrEndOfServiceService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public HrEndOfServiceController(IHrEndOfServiceService service, IHrEmployeeService empService, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrEndOfService", "View")]
    public async Task<IActionResult> Index()
    {
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
        return View(await _service.GetAllAsync());
    }

    [HasPermission("HrEndOfService", "View")]
    public async Task<IActionResult> Details(int id) => View(await _service.GetByIdAsync(id));

    [HasPermission("HrEndOfService", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Employees = await _empService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("HrEndOfService", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrEndOfServiceDto dto) { await _service.CreateAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("End of Service", "An end of service request has been created", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrEndOfService", "View")]
    public async Task<IActionResult> Calculate(int employeeId, EndOfServiceType type, DateTime effectiveDate)
        => Json(await _service.CalculateSettlementAsync(employeeId, type, effectiveDate));

    [HttpPost, HasPermission("HrEndOfService", "Edit")]
    public async Task<IActionResult> Approve(int id) { await _service.ApproveAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Index)); }

    [HttpPost, HasPermission("HrEndOfService", "Edit")]
    public async Task<IActionResult> MarkSettled(int id, string paymentMethod, string paymentReference)
    {
        await _service.MarkSettledAsync(id, paymentMethod, paymentReference);
        return RedirectToAction(nameof(Index));
    }
}
