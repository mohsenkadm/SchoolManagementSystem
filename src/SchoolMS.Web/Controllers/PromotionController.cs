using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class PromotionController : Controller
{
    private readonly IPromotionService _service;
    private readonly IAcademicYearService _yearService;
    private readonly IClassRoomService _classRoomService;
    private readonly IStudentService _studentService;
    private readonly IOneSignalNotificationService _pushService;

    public PromotionController(IPromotionService service, IAcademicYearService yearService,
        IClassRoomService classRoomService, IStudentService studentService, IOneSignalNotificationService pushService)
    {
        _service = service; _yearService = yearService;
        _classRoomService = classRoomService; _studentService = studentService; _pushService = pushService;
    }

    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    [HasPermission("Promotion", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Student Promotion";
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        return View();
    }

    [HttpPost, HasPermission("Promotion", "View"), ValidateAntiForgeryToken]
    public async Task<IActionResult> GetStudents([FromBody] PromotionFilterDto filter)
    {
        var previews = await _service.PreviewPromotionAsync(filter.FromClassRoomId, 0, 0);
        return Ok(previews);
    }

    [HttpPost, HasPermission("Promotion", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Promote([FromBody] PromotionExecuteDto dto)
    {
        var allStudents = await _studentService.GetAllAsync();
        var promotions = allStudents
            .Where(s => dto.StudentIds.Contains(s.Id))
            .Select(s => new StudentPromotionDto
            {
                StudentId = s.Id,
                StudentName = s.FullName,
                FromClassRoomId = s.ClassRoomId,
                ToClassRoomId = dto.ToClassRoomId,
                FromAcademicYearId = s.AcademicYearId,
                ToAcademicYearId = dto.ToAcademicYearId,
                Status = Domain.Enums.PromotionStatus.Promoted
            })
            .ToList();
        await _service.ExecutePromotionAsync(promotions);
        if (CurrentSchoolId.HasValue)
            await _pushService.SendToClassRoomAsync("Students Promoted",
                $"{promotions.Count} students have been promoted",
                new[] { "Parent", "Student" }, CurrentSchoolId.Value, dto.ToClassRoomId);
        return Ok(new { count = promotions.Count });
    }
}
