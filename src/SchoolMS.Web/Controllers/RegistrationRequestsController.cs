using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.Web.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class RegistrationRequestsController : Controller
{
    private readonly IRegistrationRequestService _service;
    private readonly IOneSignalNotificationService _pushService;

    public RegistrationRequestsController(IRegistrationRequestService service, IOneSignalNotificationService pushService)
    {
        _service = service;
        _pushService = pushService;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Registration Requests";
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _service.GetAllAsync();
        return Json(new { data = requests });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var request = await _service.GetByIdAsync(id);
        if (request == null) return NotFound();
        return Json(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus([FromBody] UpdateRequestStatusDto dto)
    {
        await _service.UpdateStatusAsync(dto);
        var request = await _service.GetByIdAsync(dto.Id);
        if (request?.CreatedSchoolId.HasValue == true)
            await _pushService.SendToSchoolAsync("Registration Request Updated",
                $"Registration for {request.SchoolName} has been updated to {dto.Status}",
                request.CreatedSchoolId.Value);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Stats()
    {
        var stats = await _service.GetStatsAsync();
        return Json(stats);
    }
}
