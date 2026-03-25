using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IChatService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly IOneSignalNotificationService _pushService;

    public ChatController(IChatService service, IBranchService branchService,
        IPlatformService platformService, IClassRoomService classRoomService, ISubjectService subjectService,
        ITeacherService teacherService, IOneSignalNotificationService pushService)
    {
        _service = service; _branchService = branchService;
        _platformService = platformService; _classRoomService = classRoomService; _subjectService = subjectService;
        _teacherService = teacherService; _pushService = pushService;
    }

    private bool IsSuperAdmin => User.IsInRole("SuperAdmin");
    private bool IsAdmin => User.IsInRole("SuperAdmin") || User.IsInRole("SchoolAdmin");
    private int? CurrentSchoolId { get { var c = User.FindFirst("SchoolId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }
    private int? CurrentBranchId { get { var c = User.FindFirst("BranchId"); return c != null && int.TryParse(c.Value, out var id) ? id : null; } }

    [HasPermission("Chat", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Chat";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.IsAdmin = IsAdmin;

        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            ViewBag.Branches = new List<BranchDto>();
            ViewBag.Subjects = new List<SubjectDto>();
            ViewBag.ClassRooms = new List<ClassRoomDto>();
            ViewBag.Teachers = new List<TeacherDto>();
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
            ViewBag.Subjects = CurrentSchoolId.HasValue
                ? await _subjectService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<SubjectDto>();
            ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
            ViewBag.Teachers = CurrentSchoolId.HasValue
                ? await _teacherService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<TeacherDto>();
        }

        ViewBag.CurrentBranchId = CurrentBranchId;
        ViewBag.CurrentSchoolId = CurrentSchoolId;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms(int? schoolId, int? branchId)
    {
        if (IsSuperAdmin)
        {
            if (branchId.HasValue)
                return Json(await _service.GetRoomsByBranchIdAsync(branchId.Value));
            if (schoolId.HasValue)
                return Json(await _service.GetRoomsBySchoolIdAsync(schoolId.Value));
            return Json(await _service.GetAllRoomsAsync());
        }
        else
        {
            var bid = branchId ?? CurrentBranchId;
            if (bid.HasValue)
                return Json(await _service.GetRoomsByBranchIdAsync(bid.Value));
            if (CurrentSchoolId.HasValue)
                return Json(await _service.GetRoomsBySchoolIdAsync(CurrentSchoolId.Value));
            return Json(await _service.GetAllRoomsAsync());
        }
    }

    [HttpPost, HasPermission("Chat", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom([FromBody] ChatRoomDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;
        var result = await _service.CreateRoomAsync(dto);
        await _pushService.SendToSchoolAsync("New Chat Room", $"{dto.RoomName} has been created", dto.SchoolId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(int roomId) => Json(await _service.GetMessagesAsync(roomId));

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    [HttpGet]
    public async Task<IActionResult> GetClassRoomsByBranch(int branchId)
    {
        var all = await _classRoomService.GetAllAsync();
        var filtered = all.Where(c => c.BranchId == branchId).ToList();
        return Json(filtered.Select(c => new { c.Id, Name = $"{c.GradeName} - {c.DivisionName}" }));
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjectsBySchool(int schoolId)
    {
        var subjects = await _subjectService.GetBySchoolIdAsync(schoolId);
        return Json(subjects.Select(s => new { s.Id, Name = s.SubjectName }));
    }

    [HttpGet]
    public async Task<IActionResult> GetTeachersBySchool(int schoolId)
    {
        var teachers = await _teacherService.GetBySchoolIdAsync(schoolId);
        return Json(teachers.Select(t => new { t.Id, t.FullName }));
    }
}
