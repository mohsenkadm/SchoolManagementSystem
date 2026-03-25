using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ExamScheduleController : Controller
{
    private readonly IExamScheduleService _service;
    private readonly IExamTypeService _examTypeService;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public ExamScheduleController(IExamScheduleService service, IExamTypeService examTypeService,
        IClassRoomService classRoomService, ISubjectService subjectService,
        ITeacherService teacherService, IAcademicYearService yearService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _examTypeService = examTypeService; _classRoomService = classRoomService;
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

    [HasPermission("ExamSchedule", "View")]
    public async Task<IActionResult> Index(int? academicYearId = null, int? examTypeId = null,
        int? subjectId = null, int? teacherId = null, int? classRoomId = null)
    {
        ViewData["Title"] = "Exam Schedule";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.AcademicYears = IsSuperAdmin
            ? await _yearService.GetAllAsync()
            : await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.ExamTypes = await _examTypeService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.SelectedAcademicYearId = academicYearId;
        ViewBag.SelectedExamTypeId = examTypeId;

        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value, examTypeId: examTypeId,
                    classRoomId: classRoomId, subjectId: subjectId, teacherId: teacherId, academicYearId: academicYearId)
                : new List<ExamScheduleDto>();
        return View(all);
    }

    [HasPermission("ExamSchedule", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Exam";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("ExamSchedule", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamScheduleDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToClassRoomAsync("New Exam Scheduled",
            $"Exam for {dto.SubjectName ?? "a subject"} on {dto.ExamDate:d}",
            new[] { "Parent", "Teacher", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("ExamSchedule", "Add")]
    public async Task<IActionResult> BulkCreate()
    {
        ViewData["Title"] = "Bulk Add Exams";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("ExamSchedule", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkCreate([FromBody] List<ExamScheduleDto> dtos)
    {
        if (dtos == null || dtos.Count == 0) return BadRequest();
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dtos.ForEach(d => d.SchoolId = CurrentSchoolId.Value);
        await _service.CreateBulkAsync(dtos);
        return Ok();
    }

    [HasPermission("ExamSchedule", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Exam";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("ExamSchedule", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExamScheduleDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToClassRoomAsync("Exam Updated",
            $"Exam for {dto.SubjectName ?? "a subject"} on {dto.ExamDate:d} has been updated",
            new[] { "Parent", "Teacher", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("ExamSchedule", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExamSchedule.xlsx");
    }

    private async Task LoadViewBags()
    {
        ViewBag.ExamTypes = await _examTypeService.GetAllAsync();
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
    }
}
