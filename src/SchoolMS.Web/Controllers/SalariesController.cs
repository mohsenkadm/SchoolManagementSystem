using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class SalariesController : Controller
{
    private readonly ISalaryService _service;
    private readonly IPlatformService _platformService;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IOneSignalNotificationService _pushService;

    public SalariesController(ISalaryService service, IPlatformService platformService,
        ITeacherService teacherService, IStudentService studentService,
        IOneSignalNotificationService pushService)
    {
        _service = service; _platformService = platformService;
        _teacherService = teacherService; _studentService = studentService;
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

    [HasPermission("Salaries", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Salary Management";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        var all = IsSuperAdmin
            ? await _service.GetAllSetupsAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SalarySetupDto>();
        return View(all);
    }

    [HasPermission("Salaries", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Salary Setup";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.CurrentSchoolId = CurrentSchoolId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetPersonsByType(string personType, int? schoolId)
    {
        var sid = IsSuperAdmin ? schoolId : CurrentSchoolId;
        if (personType == "Teacher")
        {
            var all = await _teacherService.GetAllAsync();
            var filtered = sid.HasValue ? all.Where(t => t.SchoolId == sid.Value).ToList() : all;
            return Json(filtered.Select(t => new { id = t.Id, name = t.FullName, baseSalary = t.BaseSalary }));
        }
        return Json(new List<object>());
    }

    [HttpPost, HasPermission("Salaries", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalarySetupDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateSetupAsync(dto);
        await _pushService.SendToIndividualAsync("Salary Setup Created",
            $"A salary setup has been created for {dto.PersonName ?? "you"}",
            dto.PersonId, dto.PersonType.ToString(), dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Salaries", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetSetupByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Salary Setup";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.CurrentSchoolId = CurrentSchoolId;
        return View("Create", item);
    }

    [HttpPost, HasPermission("Salaries", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SalarySetupDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        await _service.UpdateSetupAsync(dto);
        await _pushService.SendToIndividualAsync("Salary Updated",
            $"Your salary setup has been updated",
            dto.PersonId, dto.PersonType.ToString(), dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Salaries", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteSetupAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel() { var bytes = await _service.ExportToExcelAsync(); return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Salaries.xlsx"); }
}
