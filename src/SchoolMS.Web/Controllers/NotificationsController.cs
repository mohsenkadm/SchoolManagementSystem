using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _service;
    private readonly IClassRoomService _classRoomService;
    private readonly IBranchService _branchService;
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly IOneSignalNotificationService _pushService;

    public NotificationsController(INotificationService service, IClassRoomService classRoomService,
        IBranchService branchService, IStudentService studentService, ITeacherService teacherService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _classRoomService = classRoomService;
        _branchService = branchService; _studentService = studentService; _teacherService = teacherService;
        _pushService = pushService;
    }

    [HasPermission("Notifications", "View")]
    public async Task<IActionResult> Index() { ViewData["Title"] = "Notifications"; return View(await _service.GetAllAsync()); }

    [HasPermission("Notifications", "Add")]
    public IActionResult Create() { ViewData["Title"] = "Add Notification"; return View(); }

    [HttpPost, HasPermission("Notifications", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NotificationDto dto) { await _service.CreateAsync(dto); return RedirectToAction(nameof(Index)); }

    [HttpDelete("{id}"), HasPermission("Notifications", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpPost, HasPermission("Notifications", "Edit")]
    public async Task<IActionResult> Send(int id)
    {
        var all = await _service.GetAllAsync();
        var notification = all.FirstOrDefault(n => n.Id == id);
        await _service.MarkAsSentAsync(id);
        if (notification != null)
        {
            var schoolClaim = User.FindFirst("SchoolId");
            if (schoolClaim != null && int.TryParse(schoolClaim.Value, out var schoolId))
                await _pushService.SendToSchoolAsync(notification.Title, notification.Message, schoolId);
        }
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetTargetData(string target)
    {
        if (target == "Class")
        {
            var rooms = await _classRoomService.GetAllAsync();
            return Json(rooms.Select(r => new { id = r.Id, name = r.GradeName + " - " + r.DivisionName }));
        }
        else if (target == "Branch")
        {
            var branches = await _branchService.GetAllAsync();
            return Json(branches.Select(b => new { id = b.Id, name = b.Name }));
        }
        else if (target == "Individual")
        {
            var students = await _studentService.GetAllAsync();
            var teachers = await _teacherService.GetAllAsync();
            var items = students.Select(s => new { id = s.Id, name = s.FullName + " (Student)" })
                .Concat(teachers.Select(t => new { id = t.Id, name = t.FullName + " (Teacher)" }));
            return Json(items);
        }
        return Json(new List<object>());
    }
}
