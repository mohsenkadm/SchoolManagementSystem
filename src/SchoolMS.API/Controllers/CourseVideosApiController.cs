using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SchoolMS.API.Hubs;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SubscriptionStatus = SchoolMS.Domain.Enums.SubscriptionStatus;
using UserType = SchoolMS.Domain.Enums.UserType;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة فيديوهات الكورسات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/course-videos")]
[Authorize]
public class CourseVideosApiController : ControllerBase
{
    private readonly ICourseVideoService _service;
    private readonly ICourseService _courseService;
    private readonly IBunnyStreamService _bunnyStreamService;
    private readonly IStorageQuotaService _storageQuotaService;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<StudentSubscription> _subscriptionRepo;
    private readonly IRepository<SchoolSubscription> _schoolSubscriptionRepo;
    private readonly IRepository<School> _schoolRepo;
    private readonly IOneSignalNotificationService _pushService;
    private readonly IHubContext<ApiNotificationHub> _notificationHub;
    private readonly IUnitOfWork _unitOfWork;

    public CourseVideosApiController(
        ICourseVideoService service,
        ICourseService courseService,
        IBunnyStreamService bunnyStreamService,
        IStorageQuotaService storageQuotaService,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<StudentSubscription> subscriptionRepo,
        IRepository<SchoolSubscription> schoolSubscriptionRepo,
        IRepository<School> schoolRepo,
        IOneSignalNotificationService pushService,
        IHubContext<ApiNotificationHub> notificationHub,
        IUnitOfWork unitOfWork)
    {
        _service = service;
        _courseService = courseService;
        _bunnyStreamService = bunnyStreamService;
        _storageQuotaService = storageQuotaService;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _subscriptionRepo = subscriptionRepo;
        _schoolSubscriptionRepo = schoolSubscriptionRepo;
        _schoolRepo = schoolRepo;
        _pushService = pushService;
        _notificationHub = notificationHub;
        _unitOfWork = unitOfWork;
    }

    private string UserTypeClaim => User.FindFirst("UserType")?.Value ?? "";
    private string UserName => User.FindFirst(ClaimTypes.Name)?.Value ?? "";
    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<List<CourseVideoDto>>> GetByCourse(int schoolId, int courseId)
    {
        var accessCheck = await CheckCourseAccessAsync(courseId);
        if (accessCheck != null) return accessCheck;

        if (!await IsSchoolCourseSubscriptionActiveAsync(schoolId))
            return Ok(new List<CourseVideoDto>());

        List<CourseVideoDto> items;

        // If student, return watched status per video
        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student != null)
            {
                items = await _service.GetAllByCourseForStudentAsync(courseId, student.Id);
            }
            else
            {
                items = await _service.GetAllByCourseAsync(courseId);
            }
        }
        else
        {
            items = await _service.GetAllByCourseAsync(courseId);
        }

        foreach (var item in items)
        {
            if (item.IsScheduled)
            {
                item.VideoUrl = null;
                item.BunnyStreamVideoId = null;
                item.ThumbnailUrl = null;
            }
        }
        return Ok(items);
    }

    // جلب الفيديوهات المجانية لكورس معين — لا يتطلب اشتراك
    [HttpGet("course/{courseId}/free")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CourseVideoDto>>> GetFreeByCourse(int schoolId, int courseId)
    {
        var items = await _service.GetFreeVideosByCourseAsync(courseId);
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseVideoDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        var accessCheck = await CheckCourseAccessAsync(item.CourseId);
        if (accessCheck != null) return accessCheck;

        if (!await IsSchoolCourseSubscriptionActiveAsync(schoolId))
            return NotFound(new { error = "School subscription is inactive or does not include courses." });

        if (item.IsScheduled)
        {
            item.VideoUrl = null;
            item.BunnyStreamVideoId = null;
            item.ThumbnailUrl = null;
        }
        return Ok(item);
    }

    [HttpGet("storage-quota")]
    public async Task<ActionResult<StorageQuotaDto>> GetStorageQuota(int schoolId)
    {
        var quota = await _storageQuotaService.GetQuotaAsync(schoolId);
        return quota == null ? NotFound() : Ok(quota);
    }

    [HttpPost]
    public async Task<ActionResult<CourseVideoDto>> Create(int schoolId, [FromBody] CreateCourseVideoDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            var course = await _courseService.GetByIdAsync(dto.CourseId);
            if (course == null || course.TeacherId != teacher.Id) return Forbid();
        }

        var result = await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Course Video", "A new video has been uploaded", new[] { "Student" }, schoolId);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<CourseVideoDto>> Update(int schoolId, [FromBody] CourseVideoDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            var course = await _courseService.GetByIdAsync(dto.CourseId);
            if (course == null || course.TeacherId != teacher.Id) return Forbid();
        }

        return Ok(await _service.UpdateAsync(dto));
    }

    [HttpPost("{courseVideoId}/upload")]
    [RequestSizeLimit(500_000_000)]
    public async Task<ActionResult<VideoUploadResultDto>> UploadVideo(int schoolId, int courseVideoId, IFormFile file)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        var video = await _service.GetByIdAsync(courseVideoId);
        if (video == null) return NotFound();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            var course = await _courseService.GetByIdAsync(video.CourseId);
            if (course == null || course.TeacherId != teacher.Id) return Forbid();
        }

        if (schoolId > 0)
        {
            var (allowed, error) = await _storageQuotaService.CanUploadAsync(schoolId, file.Length);
            if (!allowed)
                return BadRequest(new { error });
        }

        using var stream = file.OpenReadStream();
        var result = await _bunnyStreamService.UploadVideoAsync(stream, file.FileName);

        // If video already had a Bunny video, delete the old one
        if (!string.IsNullOrEmpty(video.BunnyStreamVideoId))
        {
            try { await _bunnyStreamService.DeleteVideoAsync(video.BunnyStreamVideoId); }
            catch { /* best-effort cleanup */ }
        }

        video.BunnyStreamVideoId = result.VideoId;
        video.VideoUrl = result.MasterPlaylistUrl;
        video.ThumbnailUrl = result.ThumbnailUrl;
        video.FileSizeBytes = file.Length;
        await _service.UpdateAsync(video);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int schoolId, int id)
    {
        if (UserTypeClaim == nameof(UserType.Student))
            return Forbid();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var video = await _service.GetByIdAsync(id);
            if (video == null) return NotFound();
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null) return Forbid();
            var course = await _courseService.GetByIdAsync(video.CourseId);
            if (course == null || course.TeacherId != teacher.Id) return Forbid();
        }

        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("{id}/seen")]
    public async Task<IActionResult> MarkSeen(int schoolId, int id, [FromBody] int studentId)
    {
        await _service.MarkSeenAsync(id, studentId);
        return Ok(new { success = true });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> ToggleLike(int schoolId, int id, [FromBody] int studentId)
    {
        var liked = await _service.ToggleLikeAsync(id, studentId);
        var video = await _service.GetByIdAsync(id);
        return Ok(new { liked, likeCount = video?.LikeCount ?? 0 });
    }

    [HttpPost("{id}/rate")]
    public async Task<IActionResult> RateVideo(int schoolId, int id, [FromBody] VideoRateRequestDto request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new { error = "Rating must be between 1 and 5." });

        var averageRating = await _service.RateVideoAsync(id, request.StudentId, request.Rating);
        var video = await _service.GetByIdAsync(id);
        return Ok(new { averageRating, ratingCount = video?.RatingCount ?? 0 });
    }

    // جلب تعليقات فيديو كورس معين
    [HttpGet("{courseVideoId}/comments")]
    public async Task<ActionResult<List<VideoCommentDto>>> GetComments(int schoolId, int courseVideoId)
        => Ok(await _service.GetCommentsByVideoIdAsync(courseVideoId));

    // إضافة تعليق على فيديو كورس (فقط الطالب)
    [HttpPost("{courseVideoId}/comments")]
    public async Task<ActionResult<VideoCommentDto>> AddComment(int schoolId, int courseVideoId, [FromBody] CreateVideoCommentDto dto)
    {
        if (UserTypeClaim != nameof(UserType.Student))
            return Forbid();

        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
        if (student == null) return Forbid();

        dto.CourseVideoId = courseVideoId;
        dto.StudentId = student.Id;

        var result = await _service.AddCommentAsync(dto);
        result.StudentName = student.FullName;

        // إرسال إشعار للمدرس صاحب الكورس
        var videoInfo = await _service.GetByIdAsync(courseVideoId);
        if (videoInfo != null)
        {
            var course = await _courseService.GetByIdAsync(videoInfo.CourseId);
            if (course != null)
            {
                var teacher = await _teacherRepo.GetByIdAsync(course.TeacherId);
                if (teacher != null)
                {
                    await _notificationHub.Clients.All.SendAsync("ReceiveNotification",
                        "New Comment", $"{student.FullName} commented on {videoInfo.Title}", DateTime.UtcNow);
                    await _pushService.SendToIndividualAsync("New Comment",
                        $"{student.FullName} commented on {videoInfo.Title}",
                        teacher.Id, "Teacher", schoolId);
                }
            }
        }

        return Ok(result);
    }

    // ===== Video Quiz Endpoints =====

    [HttpGet("{courseVideoId}/quiz")]
    public async Task<ActionResult<List<VideoQuizQuestionDto>>> GetQuizQuestions(int schoolId, int courseVideoId)
        => Ok(await _service.GetQuizQuestionsByVideoAsync(courseVideoId));

    [HttpPost("{courseVideoId}/quiz")]
    public async Task<ActionResult<VideoQuizQuestionDto>> CreateQuizQuestion(int schoolId, int courseVideoId, [FromBody] CreateVideoQuizQuestionDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student)) return Forbid();
        dto.CourseVideoId = courseVideoId;
        return Ok(await _service.CreateQuizQuestionAsync(dto));
    }

    [HttpPut("{courseVideoId}/quiz/{questionId}")]
    public async Task<ActionResult<VideoQuizQuestionDto>> UpdateQuizQuestion(int schoolId, int courseVideoId, int questionId, [FromBody] VideoQuizQuestionDto dto)
    {
        if (UserTypeClaim == nameof(UserType.Student)) return Forbid();
        dto.Id = questionId;
        dto.CourseVideoId = courseVideoId;
        return Ok(await _service.UpdateQuizQuestionAsync(dto));
    }

    [HttpDelete("{courseVideoId}/quiz/{questionId}")]
    public async Task<IActionResult> DeleteQuizQuestion(int schoolId, int courseVideoId, int questionId)
    {
        if (UserTypeClaim == nameof(UserType.Student)) return Forbid();
        await _service.DeleteQuizQuestionAsync(questionId);
        return Ok();
    }

    [HttpPost("{courseVideoId}/quiz/submit")]
    public async Task<ActionResult<VideoQuizAnswerDto>> SubmitQuizAnswer(int schoolId, int courseVideoId, [FromBody] SubmitVideoQuizAnswerDto dto)
    {
        if (UserTypeClaim != nameof(UserType.Student)) return Forbid();
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
        if (student == null) return Forbid();
        dto.StudentId = student.Id;
        return Ok(await _service.SubmitQuizAnswerAsync(dto));
    }

    [HttpGet("{courseVideoId}/quiz/answers")]
    public async Task<ActionResult<List<VideoQuizAnswerDto>>> GetAllAnswers(int schoolId, int courseVideoId)
    {
        if (UserTypeClaim == nameof(UserType.Student)) return Forbid();
        return Ok(await _service.GetAllAnswersByVideoAsync(courseVideoId));
    }

    [HttpGet("{courseVideoId}/quiz/my-answers")]
    public async Task<ActionResult<List<VideoQuizAnswerDto>>> GetMyAnswers(int schoolId, int courseVideoId)
    {
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
        if (student == null) return Forbid();
        return Ok(await _service.GetStudentAnswersAsync(courseVideoId, student.Id));
    }

    // ===== Video Notes Endpoints =====

    [HttpGet("{courseVideoId}/notes")]
    public async Task<ActionResult<VideoNoteDto>> GetNote(int schoolId, int courseVideoId)
    {
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
        if (student == null) return Forbid();
        var note = await _service.GetNoteAsync(courseVideoId, student.Id);
        return note == null ? Ok(new VideoNoteDto { CourseVideoId = courseVideoId, StudentId = student.Id }) : Ok(note);
    }

    [HttpPost("{courseVideoId}/notes")]
    public async Task<ActionResult<VideoNoteDto>> SaveNote(int schoolId, int courseVideoId, [FromBody] SaveVideoNoteDto dto)
    {
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
        if (student == null) return Forbid();
        dto.CourseVideoId = courseVideoId;
        dto.StudentId = student.Id;
        return Ok(await _service.SaveNoteAsync(dto));
    }

   

    private async Task<ActionResult?> CheckCourseAccessAsync(int courseId)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        if (UserTypeClaim == nameof(UserType.Teacher))
        {
            var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == UserName);
            if (teacher == null || course.TeacherId != teacher.Id) return Forbid();
        }

        if (UserTypeClaim == nameof(UserType.Student))
        {
            var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.Username == UserName);
            if (student == null) return Forbid();
            var subjectIds = await GetApprovedSubjectIdsAsync(student.Id);
            if (!subjectIds.Contains(course.SubjectId)) return Forbid();
        }

        return null;
    }

    private async Task<List<int>> GetApprovedSubjectIdsAsync(int studentId)
    {
        return await _subscriptionRepo.Query()
            .Where(s => s.StudentId == studentId
                && s.Status == SubscriptionStatus.Approved
                && s.EndDate >= DateTime.UtcNow)
            .Include(s => s.OnlineSubscriptionPlan)
            .Select(s => s.OnlineSubscriptionPlan.SubjectId)
            .Distinct()
            .ToListAsync();
    }

    private async Task<bool> IsSchoolCourseSubscriptionActiveAsync(int schoolId)
    {
        var school = await _schoolRepo.Query().FirstOrDefaultAsync(s => s.Id == schoolId);
        if (school == null || !school.IsActive) return false;

        var activeSub = await _schoolSubscriptionRepo.Query()
            .Include(s => s.SystemSubscriptionPlan)
            .FirstOrDefaultAsync(s => s.SchoolId == schoolId && s.IsActive && s.ExpiryDate >= DateTime.UtcNow);

        if (activeSub == null) return false;
        return activeSub.SystemSubscriptionPlan.IncludesCourses;
    }
}
