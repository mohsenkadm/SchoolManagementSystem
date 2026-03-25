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

    // ========== مجموعات الاختبارات (Quiz Groups) ==========

    // جلب جميع مجموعات الاختبارات للمدرسة مع فلاتر
    [HttpGet]
    public async Task<ActionResult<List<QuizGroupDto>>> GetAll(int schoolId,
        [FromQuery] int? branchId = null, [FromQuery] int? teacherId = null,
        [FromQuery] int? subjectId = null, [FromQuery] int? academicYearId = null,
        [FromQuery] int? classRoomId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId, teacherId, subjectId, academicYearId, classRoomId));

    // جلب مجموعة اختبار بالمعرف
    [HttpGet("{id}")]
    public async Task<ActionResult<QuizGroupDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetGroupByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    // إنشاء مجموعة اختبار جديدة
    [HttpPost]
    public async Task<ActionResult<QuizGroupDto>> CreateGroup(int schoolId, [FromBody] QuizGroupDto dto)
    { var r = await _service.CreateGroupAsync(dto); await _pushService.SendToPersonTypesAsync("New Quiz", "A new quiz has been published", new[] { "Student", "Parent" }, schoolId); return Ok(r); }

    // حذف مجموعة اختبار
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(int schoolId, int id) { await _service.DeleteGroupAsync(id); return Ok(); }

    // ========== الأسئلة (Questions) ==========

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

    // ========== اختبارات الطالب ==========

    // جلب مجموعات الاختبارات للطالب - حسب فصل الطالب مع فلاتر
    [HttpGet("student")]
    public async Task<ActionResult<List<QuizGroupDto>>> GetStudentQuizzes(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] int? teacherId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Id == studentId && s.SchoolId == schoolId);
        if (student == null) return NotFound();
        return Ok(await _service.GetByClassRoomIdsAsync(new List<int> { student.ClassRoomId }, schoolId, subjectId, teacherId));
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

    // الطالب يرى إجاباته على اختبار معين
    [HttpGet("{groupId}/my-answers")]
    public async Task<ActionResult<List<QuizAnswerDto>>> GetMyAnswers(int schoolId, int groupId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        return Ok(await _service.GetStudentAnswersAsync(groupId, studentId));
    }

    // ========== اختبارات المعلم ==========

    // جلب مجموعات الاختبارات للمعلم - حسب الفصول المعينة له مع فلاتر
    [HttpGet("teacher")]
    public async Task<ActionResult<List<QuizGroupDto>>> GetTeacherQuizzes(int schoolId,
        [FromQuery] int? subjectId = null, [FromQuery] int? classRoomId = null,
        [FromQuery] int? academicYearId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();

        var items = await _service.GetBySchoolIdAsync(schoolId, teacherId: teacherId,
            subjectId: subjectId, academicYearId: academicYearId, classRoomId: classRoomId);
        return Ok(items);
    }

    // المعلم يجلب أسئلة اختبار معين
    [HttpGet("teacher/{groupId}/questions")]
    public async Task<ActionResult<List<QuizQuestionDto>>> GetTeacherQuestions(int schoolId, int groupId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();

        var group = await _service.GetGroupByIdAsync(groupId);
        if (group == null) return NotFound();
        if (group.TeacherId != teacherId) return Forbid();

        return Ok(await _service.GetQuestionsByGroupIdAsync(groupId));
    }

    // المعلم يرى جميع إجابات الطلاب على اختبار مع فلتر الفصل
    [HttpGet("teacher/{groupId}/answers")]
    public async Task<ActionResult<List<QuizAnswerDto>>> GetTeacherGroupAnswers(int schoolId, int groupId,
        [FromQuery] int? classRoomId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Teacher") return Forbid();
        var teacherId = GetPersonIdFromToken();

        var group = await _service.GetGroupByIdAsync(groupId);
        if (group == null) return NotFound();
        if (group.TeacherId != teacherId) return Forbid();

        return Ok(await _service.GetAllAnswersByGroupAsync(groupId, classRoomId));
    }

    // المعلم يرى إجابات طالب معين لاختبار معين
    [HttpGet("{groupId}/answers/{studentId}")]
    public async Task<ActionResult<List<QuizAnswerDto>>> GetStudentAnswers(int schoolId, int groupId, int studentId)
        => Ok(await _service.GetStudentAnswersAsync(groupId, studentId));

    // ========== اختبارات أبناء ولي الأمر ==========

    // جلب مجموعات الاختبارات لأبناء ولي الأمر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<QuizGroupDto>>> GetParentChildrenQuizzes(int schoolId,
        [FromQuery] int? subjectId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();

        var classRoomIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.ClassRoomId)
            .Distinct()
            .ToListAsync();

        return Ok(await _service.GetByClassRoomIdsAsync(classRoomIds, schoolId, subjectId));
    }

    // ولي الأمر يرى إجابات ابنه على اختبار معين
    [HttpGet("parent/{groupId}/answers/{studentId}")]
    public async Task<ActionResult<List<QuizAnswerDto>>> GetParentChildAnswers(int schoolId, int groupId, int studentId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();

        // Verify student is child of parent
        var isChild = await _studentRepo.Query()
            .AnyAsync(s => s.Id == studentId && s.ParentId == parentId && s.SchoolId == schoolId);
        if (!isChild) return Forbid();

        return Ok(await _service.GetStudentAnswersAsync(groupId, studentId));
    }
}
