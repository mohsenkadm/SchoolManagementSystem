using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly IOneSignalNotificationService _pushService;

    public CourseVideosApiController(
        ICourseVideoService service,
        ICourseService courseService,
        IBunnyStreamService bunnyStreamService,
        IStorageQuotaService storageQuotaService,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<StudentSubscription> subscriptionRepo,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _courseService = courseService;
        _bunnyStreamService = bunnyStreamService;
        _storageQuotaService = storageQuotaService;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _subscriptionRepo = subscriptionRepo;
        _pushService = pushService;
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

        var items = await _service.GetAllByCourseAsync(courseId);
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

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseVideoDto>> Get(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        var accessCheck = await CheckCourseAccessAsync(item.CourseId);
        if (accessCheck != null) return accessCheck;

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
}
