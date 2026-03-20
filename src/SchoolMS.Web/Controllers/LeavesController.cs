using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class LeavesController : Controller
{
    private readonly ILeaveService _service;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IPlatformService _platformService;
    private readonly IStaffService _staffService;
    private readonly IOneSignalNotificationService _pushService;

    public LeavesController(ILeaveService service, ITeacherService teacherService,
        IStudentService studentService, IPlatformService platformService, IStaffService staffService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _teacherService = teacherService;
        _studentService = studentService; _platformService = platformService; _staffService = staffService;
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

    [HasPermission("Leaves", "View")]
    public async Task<IActionResult> Index() { ViewData["Title"] = "Leave Requests"; return View(await _service.GetAllAsync()); }

    [HasPermission("Leaves", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Leave Request";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.CurrentSchoolId = CurrentSchoolId;
        return View();
    }

    [HttpPost, HasPermission("Leaves", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveRequestDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Leave Request",
            $"{dto.PersonName ?? "Someone"} requested leave from {dto.StartDate:d} to {dto.EndDate:d}",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Leaves", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Leave Request";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.CurrentSchoolId = CurrentSchoolId;
        return View("Create", item);
    }

    [HttpPost, HasPermission("Leaves", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(LeaveRequestDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto); return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Leaves", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost, HasPermission("Leaves", "Edit")]
    public async Task<IActionResult> Approve(int id)
    {
        var item = await _service.GetByIdAsync(id);
        await _service.ApproveAsync(id);
        if (item != null)
            await _pushService.SendToIndividualAsync("Leave Approved",
                $"Your leave request from {item.StartDate:d} to {item.EndDate:d} has been approved",
                item.PersonId, item.PersonType.ToString(), item.SchoolId);
        return Ok();
    }

    [HttpPost, HasPermission("Leaves", "Edit")]
    public async Task<IActionResult> Reject(int id)
    {
        var item = await _service.GetByIdAsync(id);
        await _service.RejectAsync(id);
        if (item != null)
            await _pushService.SendToIndividualAsync("Leave Rejected",
                $"Your leave request from {item.StartDate:d} to {item.EndDate:d} has been rejected",
                item.PersonId, item.PersonType.ToString(), item.SchoolId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonsByType(string personType, int? schoolId)
    {
        var sid = IsSuperAdmin ? schoolId : CurrentSchoolId;
        if (personType == "Teacher")
        {
            var all = await _teacherService.GetAllAsync();
            var filtered = sid.HasValue ? all.Where(t => t.SchoolId == sid.Value).ToList() : all;
            return Json(filtered.Select(t => new { id = t.Id, name = t.FullName }));
        }
        else if (personType == "Student")
        {
            var all = await _studentService.GetAllAsync();
            var filtered = sid.HasValue ? all.Where(s => s.SchoolId == sid.Value).ToList() : all;
            return Json(filtered.Select(s => new { id = s.Id, name = s.FullName }));
        }
        else if (personType == "Staff")
        {
            var all = await _staffService.GetAllAsync();
            var filtered = sid.HasValue ? all.Where(s => s.SchoolId == sid.Value).ToList() : all;
            return Json(filtered.Select(s => new { id = s.Id, name = s.FullName }));
        }
        return Json(new List<object>());
    }
}
