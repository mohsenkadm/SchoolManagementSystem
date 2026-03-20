using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class CarouselController : Controller
{
    private readonly ICarouselService _service;
    private readonly IBranchService _branchService;
    private readonly IPlatformService _platformService;
    private readonly IWebHostEnvironment _env;
    private readonly IOneSignalNotificationService _pushService;

    public CarouselController(
        ICarouselService service,
        IBranchService branchService,
        IPlatformService platformService,
        IWebHostEnvironment env,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _branchService = branchService;
        _platformService = platformService;
        _env = env;
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

    [HasPermission("Carousel", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Carousel Images";
        ViewBag.IsSuperAdmin = IsSuperAdmin;

        if (IsSuperAdmin)
        {
            ViewBag.Schools = await _platformService.GetAllSchoolsAsync();
            return View(await _service.GetAllAsync());
        }
        else
        {
            ViewBag.Schools = new List<SchoolDto>();
            var items = CurrentSchoolId.HasValue
                ? await _service.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<CarouselImageDto>();
            return View(items);
        }
    }

    [HasPermission("Carousel", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Image";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Carousel", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarouselImageDto dto, IFormFile? ImageFile)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;

        if (ImageFile != null && ImageFile.Length > 0)
            dto.ImageUrl = await SaveImageAsync(ImageFile);

        if (string.IsNullOrEmpty(dto.ImageUrl))
        {
            TempData["Error"] = "Please select an image.";
            await LoadViewBags(dto.SchoolId > 0 ? dto.SchoolId : null);
            return View(dto);
        }

        await _service.CreateAsync(dto);
        await _pushService.SendToSchoolAsync("New Carousel Image", "A new carousel image has been added", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Carousel", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var items = await _service.GetAllAsync();
        var item = items.FirstOrDefault(x => x.Id == id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Image";
        await LoadViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Carousel", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CarouselImageDto dto, IFormFile? ImageFile)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;

        if (ImageFile != null && ImageFile.Length > 0)
            dto.ImageUrl = await SaveImageAsync(ImageFile);

        await _service.UpdateAsync(dto);
        await _pushService.SendToSchoolAsync("Carousel Updated", "A carousel image has been updated", dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Carousel", "Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    private async Task LoadViewBags(int? editSchoolId = null)
    {
        ViewBag.IsSuperAdmin = IsSuperAdmin;
        ViewBag.Schools = IsSuperAdmin
            ? await _platformService.GetAllSchoolsAsync()
            : new List<SchoolDto>();

        if (IsSuperAdmin)
        {
            ViewBag.Branches = editSchoolId.HasValue && editSchoolId.Value > 0
                ? await _branchService.GetBySchoolIdAsync(editSchoolId.Value)
                : new List<BranchDto>();
        }
        else
        {
            ViewBag.Branches = CurrentSchoolId.HasValue
                ? await _branchService.GetBySchoolIdAsync(CurrentSchoolId.Value)
                : new List<BranchDto>();
        }
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "carousel");
        Directory.CreateDirectory(uploadsDir);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/carousel/{fileName}";
    }
}
