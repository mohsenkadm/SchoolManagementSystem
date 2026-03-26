using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Application.Settings;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class ParentsController : Controller
{
    private readonly IParentService _service;
    private readonly IPlatformService _platformService;
    private readonly IBranchService _branchService;
    private readonly IRepository<Student> _studentRepository;
    private readonly IWebHostEnvironment _env;
    private readonly IOneSignalNotificationService _pushService;

    public ParentsController(
        IParentService service,
        IPlatformService platformService,
        IBranchService branchService,
        IRepository<Student> studentRepository,
        IWebHostEnvironment env,
        IOneSignalNotificationService pushService)
    {
        _service = service;
        _platformService = platformService;
        _branchService = branchService;
        _studentRepository = studentRepository;
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

    [HasPermission("Students", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Parents";
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
                : new List<ParentDto>();
            return View(items);
        }
    }

    [HasPermission("Students", "Add")]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Parent";
        await LoadViewBags();
        return View();
    }

    [HttpPost, HasPermission("Students", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ParentDto dto, IFormFile? ProfileImageFile)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;

        if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            dto.ProfileImage = await SaveImageAsync(ProfileImageFile);

        await _service.CreateAsync(dto);
        await _pushService.SendToPersonTypesAsync("New Parent Added",
            $"{dto.FatherName} has been registered",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HasPermission("Students", "Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        ViewData["Title"] = "Edit Parent";
        await LoadViewBags(item.SchoolId);
        return View("Create", item);
    }

    [HttpPost, HasPermission("Students", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ParentDto dto, IFormFile? ProfileImageFile)
    {
        if (!IsSuperAdmin && CurrentSchoolId.HasValue)
            dto.SchoolId = CurrentSchoolId.Value;

        if (ProfileImageFile != null && ProfileImageFile.Length > 0)
            dto.ProfileImage = await SaveImageAsync(ProfileImageFile);

        await _service.UpdateAsync(dto);
        await _pushService.SendToPersonTypesAsync("Parent Updated",
            $"{dto.FatherName} record has been updated",
            new[] { "Staff" }, dto.SchoolId);
        return RedirectToAction(nameof(Index));
    }

    [HttpDelete("{id}"), HasPermission("Students", "Delete")]
    public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return Ok(); }

    [HttpGet]
    public async Task<IActionResult> GetBranchesBySchool(int schoolId)
    {
        var branches = await _branchService.GetBySchoolIdAsync(schoolId);
        return Json(branches.Select(b => new { b.Id, b.Name }));
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByBranch(int branchId)
    {
        var students = await _studentRepository.Query()
            .Where(s => s.BranchId == branchId && !s.IsDeleted)
            .Select(s => new { s.Id, s.FullName })
            .ToListAsync();
        return Json(students);
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
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "parents");
        Directory.CreateDirectory(uploadsDir);
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return AppUrlSettings.BuildWebUrl($"/uploads/parents/{fileName}");
    }
}
