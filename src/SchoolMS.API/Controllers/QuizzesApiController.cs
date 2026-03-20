using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الاختبارات الإلكترونية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/quizzes")]
[Authorize]
public class QuizzesApiController : ControllerBase
{
    private readonly IQuizService _service;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<TeacherAssignment> _assignmentRepo;
    private readonly IOneSignalNotificationService _pushService;

    public QuizzesApiController(
        IQuizService service,
        IRepository<Student> studentRepo,
        IRepository<TeacherAssignment> assignmentRepo,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _studentRepo = studentRepo;
        _assignmentRepo = assignmentRepo;
        _pushService = pushService;
    }

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());
    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // جلب جميع مجموعات الاختبارات للمدرسة مع فلاتر
    [HttpGet]
    public async Task<ActionResult<List<QuizGroupDto>>> GetAll(int schoolId,
        [FromQuery] int? branchId = null, [FromQuery] int? teacherId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId, teacherId));

    // جلب مجموعة اختبار بالمعرف
    [HttpGet("{id}")]
    public async Task<ActionResult<QuizGroupDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetGroupByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    // إنشاء مجموعة اختبار جديدة (للمعلم)
    [HttpPost]
    public async Task<ActionResult<QuizGroupDto>> CreateGroup(int schoolId, [FromBody] QuizGroupDto dto)
    { var r = await _service.CreateGroupAsync(dto); await _pushService.SendToPersonTypesAsync("New Quiz", "A new quiz has been published", new[] { "Student", "Parent" }, schoolId); return Ok(r); }

    // حذف مجموعة اختبار
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int schoolId, int id) { await _service.DeleteGroupAsync(id); return Ok(); }

    // جلب أسئلة اختبار معين
    [HttpGet("{groupId}/questions")]
    public async Task<ActionResult<List<QuizQuestionDto>>> GetQuestions(int schoolId, int groupId)
        => Ok(await _service.GetQuestionsByGroupIdAsync(groupId));

    // إضافة سؤال للاختبار
    [HttpPost("questions")]
    public async Task<ActionResult<QuizQuestionDto>> AddQuestion(int schoolId, [FromBody] QuizQuestionDto dto)
        => Ok(await _service.AddQuestionAsync(dto));

    // حذف سؤال من الاختبار
    [HttpDelete("questions/{id}")]
    public async Task<IActionResult> DeleteQuestion(int schoolId, int id) { await _service.DeleteQuestionAsync(id); return Ok(); }

    // اختبارات الطالب - حسب فصل الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<QuizGroupDto>>> GetStudentQuizzes(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);
        if (student == null) return NotFound();
        return Ok(await _service.GetByClassRoomIdsAsync(new List<int> { student.ClassRoomId }, schoolId));
    }

    // الطالب يجيب على أسئلة اختبار
    [HttpPost("{groupId}/submit")]
    public async Task<ActionResult<List<QuizAnswerDto>>> SubmitAnswers(int schoolId, int groupId, [FromBody] List<SubmitQuizAnswerDto> answers)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        return Ok(await _service.SubmitAnswersAsync(groupId, studentId, answers, schoolId));
    }

    // المعلم يرى إجابات طالب معين لاختبار معين
    [HttpGet("{groupId}/answers/{studentId}")]
    public async Task<ActionResult<List<QuizAnswerDto>>> GetStudentAnswers(int schoolId, int groupId, int studentId)
        => Ok(await _service.GetStudentAnswersAsync(groupId, studentId));
}
