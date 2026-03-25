using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireOnlinePlatform]
public class TeacherEarningsController : Controller
{
    private readonly ITeacherEarningService _service;
    private readonly ITeacherService _teacherService;
    private readonly IPlatformService _platformService;

    public TeacherEarningsController(
        ITeacherEarningService service,
        ITeacherService teacherService,
        IPlatformService platformService)
    {
        _service = service;
        _teacherService = teacherService;
        _platformService = platformService;
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

    [HasPermission("TeacherEarnings", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Teacher Earnings";
        ViewBag.IsSuperAdmin = IsSuperAdmin;

        if (IsSuperAdmin)
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
        else
            ViewBag.Schools = new List<SchoolDto>();

        ViewBag.Teachers = CurrentSchoolId.HasValue
            ? await _teacherService.GetBySchoolIdAsync(CurrentSchoolId.Value)
            : await _teacherService.GetAllAsync();

        var all = IsSuperAdmin
            ? await _service.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<TeacherEarningDto>();
        return View(all);
    }

    [HttpPost, HasPermission("TeacherEarnings", "Edit")]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.UpdateStatusAsync(id, TeacherEarningStatus.Approved);
        return Ok();
    }

    [HttpPost, HasPermission("TeacherEarnings", "Edit")]
    public async Task<IActionResult> MarkAsPaid(int id)
    {
        await _service.MarkAsPaidAsync(id);
        return Ok();
    }

    [HttpPost, HasPermission("TeacherEarnings", "Edit")]
    public async Task<IActionResult> SetPending(int id)
    {
        await _service.UpdateStatusAsync(id, TeacherEarningStatus.Pending);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _service.ExportToExcelAsync(CurrentSchoolId);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TeacherEarnings.xlsx");
    }
}
