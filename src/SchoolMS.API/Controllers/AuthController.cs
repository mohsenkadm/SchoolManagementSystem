using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.API.Resources;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly IStringLocalizer<ApiResource> _localizer;
    private readonly IPortalAuthService _portalAuthService;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Parent> _parentRepo;
    private readonly IRepository<HrEmployee> _staffRepo;

    public AuthController(
        IStringLocalizer<ApiResource> localizer,
        IPortalAuthService portalAuthService,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<Parent> parentRepo,
        IRepository<HrEmployee> staffRepo)
    {
        _localizer = localizer;
        _portalAuthService = portalAuthService;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _parentRepo = parentRepo;
        _staffRepo = staffRepo;
    }

    [HttpPost("teacher-login")]
    public async Task<ActionResult<PortalLoginResultDto>> TeacherLogin([FromBody] PortalLoginDto model)
    {
        var result = await _portalAuthService.TeacherLoginAsync(model);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("student-login")]
    public async Task<ActionResult<PortalLoginResultDto>> StudentLogin([FromBody] PortalLoginDto model)
    {
        var result = await _portalAuthService.StudentLoginAsync(model);
        if (result.Succeeded) return Ok(result);

        // Return detailed error message for device conflicts
        if (result.Error == "DeviceAlreadyActive" || result.Error == "DeviceIdRequired")
            return Unauthorized(new { result.Error, result.ErrorMessage, result.Succeeded });

        return Unauthorized(result);
    }

    [HttpPost("parent-login")]
    public async Task<ActionResult<PortalLoginResultDto>> ParentLogin([FromBody] PortalLoginDto model)
    {
        var result = await _portalAuthService.ParentLoginAsync(model);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("staff-login")]
    public async Task<ActionResult<PortalLoginResultDto>> StaffLogin([FromBody] PortalLoginDto model)
    {
        var result = await _portalAuthService.StaffLoginAsync(model);
        return result.Succeeded ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userType = User.FindFirst("UserType")?.Value;
        var personIdClaim = User.FindFirst("PersonId")?.Value;
        if (string.IsNullOrEmpty(userType) || string.IsNullOrEmpty(personIdClaim))
            return Unauthorized();

        var personId = int.Parse(personIdClaim);

        UserProfileDto? profile = userType switch
        {
            "Teacher" => await GetTeacherProfileAsync(personId),
            "Student" => await GetStudentProfileAsync(personId),
            "Parent" => await GetParentProfileAsync(personId),
            "Staff" => await GetStaffProfileAsync(personId),
            _ => null
        };

        return profile == null ? NotFound() : Ok(profile);
    }

    private async Task<UserProfileDto?> GetTeacherProfileAsync(int id)
    {
        var t = await _teacherRepo.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (t == null) return null;
        return new UserProfileDto
        {
            Id = t.Id, FullName = t.FullName, UserType = "Teacher", Phone = t.Phone, Email = t.Email,
            ProfileImage = t.ProfileImage, Username = t.Username, SchoolId = t.SchoolId,
            BranchId = t.BranchId, BranchName = t.Branch?.Name, Specialization = t.Specialization
        };
    }

    private async Task<UserProfileDto?> GetStudentProfileAsync(int id)
    {
        var s = await _studentRepo.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return null;
        return new UserProfileDto
        {
            Id = s.Id, FullName = s.FullName, UserType = "Student", Phone = s.Phone, Email = s.Email,
            ProfileImage = s.ProfileImage, Username = s.Username, SchoolId = s.SchoolId,
            BranchId = s.BranchId, BranchName = s.Branch?.Name, DateOfBirth = s.DateOfBirth, Gender = s.Gender,
            Address = s.Address
        };
    }

    private async Task<UserProfileDto?> GetParentProfileAsync(int id)
    {
        var p = await _parentRepo.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return null;
        return new UserProfileDto
        {
            Id = p.Id, FullName = p.FatherName, UserType = "Parent", Phone = p.FatherPhone, Email = p.FatherEmail,
            ProfileImage = p.ProfileImage, Username = p.Username, SchoolId = p.SchoolId,
            Address = p.Address
        };
    }

    private async Task<UserProfileDto?> GetStaffProfileAsync(int id)
    {
        var s = await _staffRepo.Query().Include(x => x.Branch).Include(x => x.JobTitle).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return null;
        return new UserProfileDto
        {
            Id = s.Id, FullName = s.FullName, UserType = "Staff", Phone = s.Phone,
            Email = s.Email, ProfileImage = s.ProfileImage,
            Username = s.Username, SchoolId = s.SchoolId,
            BranchId = s.BranchId, BranchName = s.Branch?.Name, Position = s.JobTitle?.TitleName
        };
    }

    /// <summary>
    /// تسجيل خروج الطالب من الجهاز الحالي — يمسح ActiveDeviceId ليتمكن من الدخول من جهاز آخر
    /// </summary>
    [Authorize]
    [HttpPost("student-logout-device")]
    public async Task<IActionResult> StudentLogoutDevice()
    {
        var userType = User.FindFirst("UserType")?.Value;
        var personIdClaim = User.FindFirst("PersonId")?.Value;

        if (userType != "Student" || string.IsNullOrEmpty(personIdClaim))
            return BadRequest(new { error = "OnlyStudents", message = "هذا الإجراء متاح فقط للطلبة." });

        var personId = int.Parse(personIdClaim);
        var success = await _portalAuthService.LogoutStudentDeviceAsync(personId);

        return success
            ? Ok(new { success = true, message = "تم تسجيل الخروج من الجهاز بنجاح. يمكنك الآن تسجيل الدخول من جهاز آخر." })
            : NotFound(new { error = "StudentNotFound", message = "الطالب غير موجود." });
    }
}
