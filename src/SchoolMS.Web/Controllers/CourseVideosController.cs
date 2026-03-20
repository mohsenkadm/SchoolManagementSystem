using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class CourseVideosController : Controller
{
    private readonly ICourseVideoService _service;
    private readonly ICourseService _courseService;
    private readonly IBunnyStreamService _bunnyStreamService;
    private readonly IStorageQuotaService _storageQuotaService;
    private readonly IOneSignalNotificationService _pushService;

    public CourseVideosController(
        ICourseVideoService service,
        ICourseService courseService,
        IBunnyStreamService bunnyStreamService,
        IStorageQuotaService storageQuotaService,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _courseService = courseService;
        _bunnyStreamService = bunnyStreamService;
        _storageQuotaService = storageQuotaService;
        _pushService = pushService;
    }

    private int? CurrentSchoolId
    {
        get
        {
            var claim = User.FindFirst("SchoolId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }

    [HasPermission("Courses", "View")]
    public async Task<IActionResult> Index(int courseId)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        ViewData["Title"] = $"Videos - {course.Title}";
        ViewBag.Course = course;

        if (CurrentSchoolId.HasValue)
            ViewBag.StorageQuota = await _storageQuotaService.GetQuotaAsync(CurrentSchoolId.Value);

        var items = await _service.GetAllByCourseAsync(courseId);
        return View(items);
    }

    [HasPermission("Courses", "Add")]
    public async Task<IActionResult> Create(int courseId)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        ViewData["Title"] = $"Add Video - {course.Title}";
        ViewBag.Course = course;

        if (CurrentSchoolId.HasValue)
            ViewBag.StorageQuota = await _storageQuotaService.GetQuotaAsync(CurrentSchoolId.Value);

        return View();
    }

    [HttpPost, HasPermission("Courses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseVideoDto dto)
    {
            var result = await _service.CreateAsync(dto);
            var course = await _courseService.GetByIdAsync(dto.CourseId);
            if (course != null)
                await _pushService.SendToPersonTypesAsync("New Video",
                    $"{dto.Title} added to {course.Title}",
                    new[] { "Student" }, course.SchoolId);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { id = result.Id });
            return RedirectToAction(nameof(Index), new { courseId = dto.CourseId });

    }

    [HttpGet]
    public async Task<IActionResult> GetLatestVideoId(int courseId)
    {
        var videos = await _service.GetAllByCourseAsync(courseId);
        var latest = videos.OrderByDescending(v => v.Id).FirstOrDefault();
        return latest != null ? Json(new { id = latest.Id }) : NotFound();
    }

    [HasPermission("Courses", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        var course = await _courseService.GetByIdAsync(item.CourseId);
        ViewData["Title"] = $"Edit Video - {course?.Title}";
        ViewBag.Course = course;

        if (CurrentSchoolId.HasValue)
            ViewBag.StorageQuota = await _storageQuotaService.GetQuotaAsync(CurrentSchoolId.Value);

        return View("Create", item);
    }

    [HttpPost, HasPermission("Courses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CourseVideoDto dto)
    {
        await _service.UpdateAsync(dto);
        return RedirectToAction(nameof(Index), new { courseId = dto.CourseId });
    }

    [HttpPost, HasPermission("Courses", "Add"), ValidateAntiForgeryToken]
    [RequestSizeLimit(500_000_000)]
    public async Task<IActionResult> UploadVideo(int courseVideoId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        if (CurrentSchoolId.HasValue)
        {
            var (allowed, error) = await _storageQuotaService.CanUploadAsync(CurrentSchoolId.Value, file.Length);
            if (!allowed)
                return BadRequest(new { error });
        }

        var video = await _service.GetByIdAsync(courseVideoId);
        if (video == null) return NotFound();

        using var stream = file.OpenReadStream();
        var result = await _bunnyStreamService.UploadVideoAsync(stream, file.FileName);

        video.BunnyStreamVideoId = result.VideoId;
        video.VideoUrl = result.MasterPlaylistUrl;
        video.ThumbnailUrl = result.ThumbnailUrl;
        video.FileSizeBytes = file.Length;
        await _service.UpdateAsync(video);

        return Ok(result);
    }

    [HttpPost, HasPermission("Courses", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSortOrder([FromBody] List<VideoSortDto> items)
    {
        foreach (var item in items)
        {
            var video = await _service.GetByIdAsync(item.Id);
            if (video != null)
            {
                video.SortOrder = item.SortOrder;
                await _service.UpdateAsync(video);
            }
        }
        return Ok();
    }

    [HttpPost, HasPermission("Courses", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckStorageQuota([FromBody] long fileSizeBytes)
    {
        if (!CurrentSchoolId.HasValue) return Ok(new { allowed = true });
        var (allowed, error) = await _storageQuotaService.CanUploadAsync(CurrentSchoolId.Value, fileSizeBytes);
        return Ok(new { allowed, error });
    }

    [HttpDelete("{id}"), HasPermission("Courses", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }
}
