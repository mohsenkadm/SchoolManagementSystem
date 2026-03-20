using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class StudentGradesController : Controller
{
    private readonly IStudentGradeService _service;
    private readonly IStudentService _studentService;
    private readonly ISubjectService _subjectService;
    private readonly IExamTypeService _examTypeService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public StudentGradesController(IStudentGradeService service, IStudentService studentService,
        ISubjectService subjectService, IExamTypeService examTypeService,
        IAcademicYearService yearService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _studentService = studentService; _subjectService = subjectService;
        _examTypeService = examTypeService; _yearService = yearService; _platformService = platformService;
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

    [HasPermission("StudentGrades", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Student Grades";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<StudentGradeDto>();
        return View(all);
    }

    [HasPermission("StudentGrades", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Grade";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("StudentGrades", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentGradeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateAsync(dto);
        await _pushService.SendToIndividualAsync("New Grade Posted",
            $"Grade for {dto.SubjectName ?? "a subject"}: {dto.Mark}/{dto.MaxMark}",
            dto.StudentId, "Student", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("StudentGrades", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Grade";
        await LoadViewBags();
        return View("Create", item);
    }

    [HttpPost, HasPermission("StudentGrades", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(StudentGradeDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateAsync(dto);
        await _pushService.SendToIndividualAsync("Grade Updated",
            $"Grade for {dto.SubjectName ?? "a subject"}: {dto.Mark}/{dto.MaxMark} has been updated",
            dto.StudentId, "Student", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("StudentGrades", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HasPermission("StudentGrades", "Add")]
    public async Task<IActionResult> BulkCreate()
    {
        ViewData["Title"] = "Bulk Add Grades";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("StudentGrades", "Add")]
    public async Task<IActionResult> BulkCreate([FromBody] List<StudentGradeDto> dtos)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            foreach (var dto in dtos) dto.SchoolId = CurrentSchoolId.Value;
        await _service.BulkCreateAsync(dtos);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentGrades.xlsx");
    }

    private async Task LoadViewBags()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Students = await _studentService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.ExamTypes = await _examTypeService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
    }
}
