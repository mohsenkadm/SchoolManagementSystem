using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrSettingsController : Controller
{
    private readonly IHrSettingsService _service;
    private readonly IOneSignalNotificationService _pushService;
    public HrSettingsController(IHrSettingsService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("HrSettings", "View")]
    public async Task<IActionResult> Index() => View(await _service.GetSettingsAsync());

    [HttpPost, HasPermission("HrSettings", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(HrSettingsDto dto)
    {
        await _service.UpdateSettingsAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("HR Settings Updated", "HR settings have been updated", new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }
}
