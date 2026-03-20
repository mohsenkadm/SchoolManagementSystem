using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly IAttendanceService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IOneSignalNotificationService _pushService;

    public AttendanceController(IAttendanceService service, IBranchService branchService, IPlatformService platformService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
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

    // ===== CHECK-IN / CHECK-OUT PAGE =====
    [HasPermission("Attendance", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Attendance";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Branches = IsSuperAdmin
            ? await _branchService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        return View();
    }

    [HttpPost, HasPermission("Attendance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveBadge([FromBody] string badgeCardNumber)
    {
        var result = await _service.ResolveBadgeAsync(badgeCardNumber);
        if (result == null) return NotFound("Badge card not found.");
        return Ok(result);
    }

    [HttpPost, HasPermission("Attendance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveBulk([FromBody] BulkAttendanceSaveDto dto)
    {
        try
        {
            var results = await _service.SaveBulkAsync(dto);
            var skipped = dto.Items.Count - results.Count;
            return Ok(new { count = results.Count, skipped });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost, HasPermission("Attendance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn([FromBody] CreateAttendanceDto dto)
    {
        try
        {
            var result = await _service.CheckInAsync(dto);
            if (result.PersonType == Domain.Enums.PersonType.Student && CurrentSchoolId.HasValue)
                await _pushService.SendToIndividualAsync("Attendance Recorded",
                    $"{result.PersonName} has checked {result.Type}",
                    result.PersonId, "Parent", CurrentSchoolId.Value);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // ===== REPORTS PAGE =====
    [HasPermission("Attendance", "View")]
    public async Task<IActionResult> Report()
    {
        ViewData["Title"] = "Attendance Report";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Branches = IsSuperAdmin
            ? await _branchService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        return View();
    }

    [HttpPost, HasPermission("Attendance", "View")]
    public async Task<IActionResult> GetReport([FromBody] AttendanceFilterDto filter)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue && !filter.SchoolId.HasValue)
            filter.SchoolId = CurrentSchoolId;
        return Ok(await _service.GetReportAsync(filter));
    }

    [HttpPost]
    public async Task<IActionResult> GetData([FromBody] DataTableRequest request) => Ok(await _service.GetDataTableAsync(request));

    [HttpGet]
    public async Task<IActionResult> ExportExcel(DateTime? dateFrom, DateTime? dateTo, int? type, int? personType, int? branchId, int? schoolId, string? search)
    {
        var filter = new AttendanceFilterDto
        {
            DateFrom = dateFrom, DateTo = dateTo,
            Type = type.HasValue ? (Domain.Enums.AttendanceType)type.Value : null,
            PersonType = personType.HasValue ? (Domain.Enums.PersonType)personType.Value : null,
            BranchId = branchId, SchoolId = schoolId, SearchValue = search
        };
        var bytes = await _service.ExportToExcelAsync(filter);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Attendance.xlsx");
    }

    [HttpPost, HasPermission("Attendance", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAbsentees([FromBody] MarkAbsenteesRequest request)
    {
        var count = await _service.MarkAbsenteesAsync(request.BranchId, request.Date);
        return Ok(new { count });
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    public class MarkAbsenteesRequest
    {
        public int BranchId { get; set; }
        public DateTime Date { get; set; }
    }
}
