using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class TeachersController : Controller
{
    private readonly ITeacherService _teacherService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public TeachersController(ITeacherService teacherService, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    {
        _teacherService = teacherService;
        _branchService = branchService;
        _platformService = platformService;
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

    [HasPermission("Teachers", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Teachers";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = new List<BranchDto>();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches);
    }

    [HttpPost]
    public async Task<IActionResult> GetData([FromBody] DataTableRequest request)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue && !request.SchoolId.HasValue)
            request.SchoolId = CurrentSchoolId;
        var result = await _teacherService.GetDataTableAsync(request);
        return Json(result);
    }

    [HasPermission("Teachers", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Teacher";
        ViewBag.Branches = await _branchService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("Teachers", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTeacherDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.Branches = await _branchService.GetAllAsync(); return View(dto); }
        await _teacherService.CreateAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("New Teacher Added",
                $"{dto.FullName} has joined the school",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Teachers", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var teacher = await _teacherService.GetByIdAsync(id);
        if (teacher == null) return NotFound();
        ViewData["Title"] = "Edit Teacher";
        ViewBag.Branches = await _branchService.GetAllAsync();
        return View("Create", teacher);
    }

    [HttpPost, HasPermission("Teachers", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTeacherDto dto)
    {
        if (!ModelState.IsValid) { ViewBag.Branches = await _branchService.GetAllAsync(); return View("Create", dto); }
        await _teacherService.UpdateAsync(dto);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToPersonTypesAsync("Teacher Updated",
                "A teacher record has been updated",
                new[] { "Staff" }, CurrentSchoolId.Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Teachers", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _teacherService.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> ExportExcel()
    {
        var bytes = await _teacherService.ExportToExcelAsync();
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Teachers.xlsx");
    }
}
