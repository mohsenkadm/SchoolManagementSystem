using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize, RequireHrModule]
public class HrTrainingController : Controller
{
    private readonly IHrTrainingService _service;
    private readonly IHrEmployeeService _empService;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public HrTrainingController(IHrTrainingService service, IHrEmployeeService empService, IBranchService branchService, IPlatformService platformService, IOneSignalNotificationService pushService)
    { _service = service; _empService = empService; _branchService = branchService; _platformService = platformService; _pushService = pushService; }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    // Programs
    [HasPermission("HrTraining", "View")]
    public async Task<IActionResult> Index()
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = await _branchService.GetAllAsync();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
        var list = CurrentSchoolId.HasValue
            ? await _service.GetProgramsBySchoolIdAsync(CurrentSchoolId.Value)
            : await _service.GetProgramsAsync();
        return View(list);
    }

    [HasPermission("HrTraining", "View")]
    public async Task<IActionResult> Details(int id) => View(await _service.GetProgramByIdAsync(id));

    [HasPermission("HrTraining", "Add")]
    public IActionResult Create() => View();

    [HttpPost, HasPermission("HrTraining", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(HrTrainingProgramDto dto) { await _service.CreateProgramAsync(dto); if (CurrentSchoolId.HasValue) await _pushService.SendToPersonTypesAsync("New Training Program", $"{dto.ProgramName} is now available", new[] { "Staff" }, CurrentSchoolId.Value); return RedirectToAction(nameof(Index)); }

    [HasPermission("HrTraining", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetProgramByIdAsync(id); if (item == null) return NotFound();
        return View("Create", item);
    }

    [HttpPost, HasPermission("HrTraining", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(HrTrainingProgramDto dto) { await _service.UpdateProgramAsync(dto); return RedirectToAction(nameof(Index)); }

    [HttpDelete("{id}"), HasPermission("HrTraining", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteProgramAsync(id); return Ok(); }

    // Enrollment
    [HasPermission("HrTraining", "View")]
    public async Task<IActionResult> Records(int? programId, int? employeeId)
        => View(CurrentSchoolId.HasValue
            ? await _service.GetRecordsBySchoolIdAsync(CurrentSchoolId.Value, programId, employeeId)
            : await _service.GetRecordsAsync(programId, employeeId));

    [HasPermission("HrTraining", "Add")]
    public async Task<IActionResult> Enroll(int programId)
    {
        ViewBag.ProgramId = programId;
        ViewBag.Employees = await _empService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("HrTraining", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(HrTrainingRecordDto dto) { await _service.EnrollEmployeeAsync(dto); return RedirectToAction(nameof(Records), new { programId = dto.TrainingProgramId }); }

    [HttpPost, HasPermission("HrTraining", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRecord(HrTrainingRecordDto dto) { await _service.UpdateRecordAsync(dto); return RedirectToAction(nameof(Records), new { programId = dto.TrainingProgramId }); }

    // Training Requests
    [HasPermission("HrTraining", "View")]
    public async Task<IActionResult> Requests(TrainingRequestStatus? status) => View(await _service.GetRequestsAsync(status));

    [HttpPost, HasPermission("HrTraining", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(HrTrainingRequestDto dto) { await _service.CreateRequestAsync(dto); return RedirectToAction(nameof(Requests)); }

    [HttpPost, HasPermission("HrTraining", "Edit")]
    public async Task<IActionResult> ApproveRequest(int id) { await _service.ApproveRequestAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Requests)); }

    [HttpPost, HasPermission("HrTraining", "Edit")]
    public async Task<IActionResult> RejectRequest(int id) { await _service.RejectRequestAsync(id, User.Identity?.Name ?? ""); return RedirectToAction(nameof(Requests)); }

    // Professional Certificates
    [HasPermission("HrTraining", "View")]
    public async Task<IActionResult> Certificates(int employeeId) => Json(await _service.GetCertificatesAsync(employeeId));

    [HttpPost, HasPermission("HrTraining", "Add")]
    public async Task<IActionResult> CreateCertificate([FromBody] HrProfessionalCertificateDto dto) => Json(await _service.CreateCertificateAsync(dto));

    [HttpDelete("certificate/{id}"), HasPermission("HrTraining", "Delete")]
    public async Task<IActionResult> DeleteCertificate(int id) { await _service.DeleteCertificateAsync(id); return Ok(); }
}
