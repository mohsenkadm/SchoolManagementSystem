using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class PortalAuthService : IPortalAuthService
{
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Parent> _parentRepo;
    private readonly IRepository<HrEmployee> _staffRepo;
    private readonly IRepository<School> _schoolRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public PortalAuthService(
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<Parent> parentRepo,
        IRepository<HrEmployee> staffRepo,
        IRepository<School> schoolRepo,
        IUnitOfWork unitOfWork,
        IConfiguration configuration)
    {
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _parentRepo = parentRepo;
        _staffRepo = staffRepo;
        _schoolRepo = schoolRepo;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<PortalLoginResultDto> TeacherLoginAsync(PortalLoginDto dto)
    {
        var teacher = await _teacherRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Username == dto.Username && t.SchoolId == dto.SchoolId && !t.IsDeleted);

        if (teacher == null || teacher.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(teacher.Id, teacher.FullName, "Teacher", teacher.SchoolId, teacher.BranchId, null);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = teacher.FullName,
            UserType = "Teacher", PersonId = teacher.Id,
            SchoolId = teacher.SchoolId, BranchId = teacher.BranchId,
            OneSignalAppId = await GetSchoolOneSignalAppIdAsync(teacher.SchoolId)
        };
    }

    public async Task<PortalLoginResultDto> StudentLoginAsync(PortalLoginDto dto)
    {
        var student = await _studentRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Username == dto.Username && s.SchoolId == dto.SchoolId && !s.IsDeleted);

        if (student == null || student.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        // Single-device enforcement: DeviceId is required for student login
        if (string.IsNullOrWhiteSpace(dto.DeviceId))
            return new PortalLoginResultDto { Error = "DeviceIdRequired", ErrorMessage = "يجب إرسال معرّف الجهاز (DeviceId) عند تسجيل الدخول." };

        // If this student already has an active device and it's different, reject login
        if (!string.IsNullOrWhiteSpace(student.ActiveDeviceId) && student.ActiveDeviceId != dto.DeviceId)
        {
            return new PortalLoginResultDto
            {
                Error = "DeviceAlreadyActive",
                ErrorMessage = "حسابك مسجّل الدخول على جهاز آخر. يُسمح بتسجيل الدخول من جهاز واحد فقط. يرجى تسجيل الخروج من الجهاز الآخر أولاً."
            };
        }

        // Store/update the active device
        student.ActiveDeviceId = dto.DeviceId;
        _studentRepo.Update(student);
        await _unitOfWork.SaveChangesAsync();

        var token = GeneratePortalToken(student.Id, student.FullName, "Student", student.SchoolId, student.BranchId, dto.DeviceId);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = student.FullName,
            UserType = "Student", PersonId = student.Id,
            SchoolId = student.SchoolId, BranchId = student.BranchId,
            ClassRoomId = student.ClassRoomId,
            OneSignalAppId = await GetSchoolOneSignalAppIdAsync(student.SchoolId)
        };
    }

    public async Task<PortalLoginResultDto> ParentLoginAsync(PortalLoginDto dto)
    {
        var parent = await _parentRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Username == dto.Username && p.SchoolId == dto.SchoolId && !p.IsDeleted);

        if (parent == null || parent.PasswordHash != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(parent.Id, parent.FatherName, "Parent", parent.SchoolId, null, null);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = parent.FatherName,
            UserType = "Parent", PersonId = parent.Id,
            SchoolId = parent.SchoolId,
            OneSignalAppId = await GetSchoolOneSignalAppIdAsync(parent.SchoolId)
        };
    }

    public async Task<PortalLoginResultDto> StaffLoginAsync(PortalLoginDto dto)
    {
        var staff = await _staffRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Username == dto.Username && s.SchoolId == dto.SchoolId && !s.IsDeleted);

        if (staff == null || staff.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(staff.Id, staff.FullName, "Staff", staff.SchoolId, staff.BranchId, null);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = staff.FullName,
            UserType = "Staff", PersonId = staff.Id,
            SchoolId = staff.SchoolId, BranchId = staff.BranchId,
            OneSignalAppId = await GetSchoolOneSignalAppIdAsync(staff.SchoolId)
        };
    }

    private string GeneratePortalToken(int personId, string fullName, string userType, int schoolId, int? branchId, string? deviceId)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, personId.ToString()),
            new(ClaimTypes.Name, fullName),
            new("FullName", fullName),
            new("UserType", userType),
            new("SchoolId", schoolId.ToString()),
            new("PersonId", personId.ToString())
        };

        if (branchId.HasValue)
            claims.Add(new Claim("BranchId", branchId.Value.ToString()));
        if (!string.IsNullOrWhiteSpace(deviceId))
            claims.Add(new Claim("DeviceId", deviceId));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> LogoutStudentDeviceAsync(int studentId)
    {
        var student = await _studentRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
        if (student == null) return false;

        student.ActiveDeviceId = null;
        _studentRepo.Update(student);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task<string?> GetSchoolOneSignalAppIdAsync(int schoolId)
    {
        var school = await _schoolRepo.Query().IgnoreQueryFilters()
            .Where(s => s.Id == schoolId && !s.IsDeleted)
            .Select(s => s.OneSignalAppId)
            .FirstOrDefaultAsync();
        return school;
    }
}
