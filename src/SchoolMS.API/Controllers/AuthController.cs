using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.API.Resources;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly SchoolDbContext _context;
    private readonly IStringLocalizer<ApiResource> _localizer;
    private readonly IPortalAuthService _portalAuthService;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Parent> _parentRepo;
    private readonly IRepository<Staff> _staffRepo;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        SchoolDbContext context,
        IStringLocalizer<ApiResource> localizer,
        IPortalAuthService portalAuthService,
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<Parent> parentRepo,
        IRepository<Staff> staffRepo)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
        _localizer = localizer;
        _portalAuthService = portalAuthService;
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _parentRepo = parentRepo;
        _staffRepo = staffRepo;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto model)
    {
        // Find school if slug provided
        School? school = null;
        if (!string.IsNullOrEmpty(model.SchoolSlug))
        {
            school = await _context.Schools.IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Slug == model.SchoolSlug && !s.IsDeleted && s.IsActive);
            if (school == null)
                return Unauthorized(new LoginResultDto { Error = _localizer["SchoolNotFound"].Value });
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsDeleted)
            return Unauthorized(new LoginResultDto { Error = _localizer["InvalidCredentials"].Value });

        if (school != null && user.SchoolId != school.Id)
            return Unauthorized(new LoginResultDto { Error = _localizer["InvalidCredentials"].Value });

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return Unauthorized(new LoginResultDto { Error = _localizer["AccountLockedOut"].Value });
            return Unauthorized(new LoginResultDto { Error = _localizer["InvalidCredentials"].Value });
        }

        var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
        var token = GenerateJwtToken(user, isSuperAdmin && school == null);
        return Ok(new LoginResultDto
        {
            Succeeded = true,
            Token = token,
            UserName = user.UserName,
            FullName = user.FullName,
            SchoolId = user.SchoolId,
            BranchId = user.BranchId
        });
    }

    private string GenerateJwtToken(ApplicationUser user, bool isSuperAdminGlobal = false)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("FullName", user.FullName),
            new("UserType", user.UserType.ToString())
        };

        // SuperAdmin without school slug → no SchoolId claim → sees all schools
        if (!isSuperAdminGlobal)
            claims.Add(new Claim("SchoolId", user.SchoolId.ToString()));

        if (user.BranchId.HasValue)
            claims.Add(new Claim("BranchId", user.BranchId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
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
        return result.Succeeded ? Ok(result) : Unauthorized(result);
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
        var s = await _staffRepo.Query().Include(x => x.Branch).FirstOrDefaultAsync(x => x.Id == id);
        if (s == null) return null;
        return new UserProfileDto
        {
            Id = s.Id, FullName = s.FullName, UserType = "Staff", Phone = s.Phone,
            Username = s.Username, SchoolId = s.SchoolId,
            BranchId = s.BranchId, BranchName = s.Branch?.Name, Position = s.Position
        };
    }
}
