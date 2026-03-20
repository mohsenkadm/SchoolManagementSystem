using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class WeeklyScheduleController : Controller
{
    private readonly IWeeklyScheduleService _service;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public WeeklyScheduleController(IWeeklyScheduleService service, IClassRoomService classRoomService,
        ISubjectService subjectService, ITeacherService teacherService, IAcademicYearService yearService,
        IPlatformService platformService, IOneSignalNotificationService pushService)
    {
        _service = service; _classRoomService = classRoomService;
        _subjectService = subjectService; _teacherService = teacherService; _yearService = yearService;
        _platformService = platformService; _pushService = pushService;
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

    [HasPermission("WeeklySchedule", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Weekly Timetable";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<WeeklyScheduleDto>();
        return View(all);
    }

    [HasPermission("WeeklySchedule", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Schedule";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("WeeklySchedule", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WeeklyScheduleDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToClassRoomAsync("New Schedule",
            $"{dto.SubjectName ?? "Subject"} - {dto.DayOfWeek} {dto.StartTime:hh\\:mm}",
            new[] { "Parent", "Teacher", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("WeeklySchedule", "Add")]
    public async Task<IActionResult> BulkCreate()
    {
        ViewData["Title"] = "Bulk Add Schedule";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("WeeklySchedule", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkCreate([FromBody] List<WeeklyScheduleDto> dtos)
    {
        if (dtos == null || dtos.Count == 0) return BadRequest();
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dtos.ForEach(d => d.SchoolId = CurrentSchoolId.Value);
        await _service.CreateBulkAsync(dtos);
        return Ok();
    }

    [HasPermission("WeeklySchedule", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Schedule";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("WeeklySchedule", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(WeeklyScheduleDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToClassRoomAsync("Schedule Updated",
            $"{dto.SubjectName ?? "Subject"} - {dto.DayOfWeek} {dto.StartTime:hh\\:mm} has been updated",
            new[] { "Parent", "Teacher", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("WeeklySchedule", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "WeeklySchedule.xlsx");
    }

    private async Task LoadViewBags()
    {
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
    }
}
