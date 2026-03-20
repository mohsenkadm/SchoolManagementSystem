using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class QuizzesController : Controller
{
    private readonly IQuizService _service;
    private readonly IClassRoomService _classRoomService;
    private readonly ISubjectService _subjectService;
    private readonly ITeacherService _teacherService;
    private readonly IAcademicYearService _yearService;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IOneSignalNotificationService _pushService;

    public QuizzesController(IQuizService service, IClassRoomService classRoomService,
        ISubjectService subjectService, ITeacherService teacherService, IAcademicYearService yearService,
        IPlatformService platformService, IBranchService branchService, IOneSignalNotificationService pushService)
    {
        _service = service; _classRoomService = classRoomService;
        _subjectService = subjectService; _teacherService = teacherService; _yearService = yearService;
        _platformService = platformService; _branchService = branchService; _pushService = pushService;
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

    [HasPermission("Quizzes", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Quizzes";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        ViewBag.Branches = IsSuperAdmin
            ? await _branchService.GetAllAsync()
            : CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        return View(await _service.GetAllGroupsAsync());
    }

    [HasPermission("Quizzes", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Quiz Group";
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin ? await _platformService.GetAllSchoolsAsync() : new List<SchoolDto>();
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Quizzes", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuizGroupDto dto)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue) dto.SchoolId = CurrentSchoolId.Value;
        await _service.CreateGroupAsync(dto);
        await _pushService.SendToClassRoomAsync("New Quiz",
            $"{dto.GroupName} - {dto.SubjectName ?? "Subject"}",
            new[] { "Parent", "Student" }, dto.SchoolId, dto.ClassRoomId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Quizzes", "Delete")]
    public async Task<IActionResult> DeleteGroup(int id) { await _service.DeleteGroupAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(int groupId) => Json(await _service.GetQuestionsByGroupIdAsync(groupId));

    [HttpPost, HasPermission("Quizzes", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion([FromBody] QuizQuestionDto dto) => Ok(await _service.AddQuestionAsync(dto));

    [HttpDelete("Question/{id}"), HasPermission("Quizzes", "Delete")]
    public async Task<IActionResult> DeleteQuestion(int id) { await _service.DeleteQuestionAsync(id); return Ok(); }

    private async Task LoadViewBags()
    {
        ViewBag.ClassRooms = await _classRoomService.GetAllAsync();
        ViewBag.Subjects = await _subjectService.GetAllAsync();
        ViewBag.Teachers = await _teacherService.GetAllAsync();
        ViewBag.AcademicYears = await _yearService.GetAllAsync(CurrentSchoolId ?? 0);
    }
}
