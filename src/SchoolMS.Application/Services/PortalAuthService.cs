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
    private readonly IRepository<Staff> _staffRepo;
    private readonly IConfiguration _configuration;

    public PortalAuthService(
        IRepository<Teacher> teacherRepo,
        IRepository<Student> studentRepo,
        IRepository<Parent> parentRepo,
        IRepository<Staff> staffRepo,
        IConfiguration configuration)
    {
        _teacherRepo = teacherRepo;
        _studentRepo = studentRepo;
        _parentRepo = parentRepo;
        _staffRepo = staffRepo;
        _configuration = configuration;
    }

    public async Task<PortalLoginResultDto> TeacherLoginAsync(PortalLoginDto dto)
    {
        var teacher = await _teacherRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Username == dto.Username && t.SchoolId == dto.SchoolId && !t.IsDeleted);

        if (teacher == null || teacher.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(teacher.Id, teacher.FullName, "Teacher", teacher.SchoolId, teacher.BranchId);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = teacher.FullName,
            UserType = "Teacher", PersonId = teacher.Id,
            SchoolId = teacher.SchoolId, BranchId = teacher.BranchId
        };
    }

    public async Task<PortalLoginResultDto> StudentLoginAsync(PortalLoginDto dto)
    {
        var student = await _studentRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Username == dto.Username && s.SchoolId == dto.SchoolId && !s.IsDeleted);

        if (student == null || student.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(student.Id, student.FullName, "Student", student.SchoolId, student.BranchId);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = student.FullName,
            UserType = "Student", PersonId = student.Id,
            SchoolId = student.SchoolId, BranchId = student.BranchId
        };
    }

    public async Task<PortalLoginResultDto> ParentLoginAsync(PortalLoginDto dto)
    {
        var parent = await _parentRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Username == dto.Username && p.SchoolId == dto.SchoolId && !p.IsDeleted);

        if (parent == null || parent.PasswordHash != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(parent.Id, parent.FatherName, "Parent", parent.SchoolId, null);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = parent.FatherName,
            UserType = "Parent", PersonId = parent.Id,
            SchoolId = parent.SchoolId
        };
    }

    public async Task<PortalLoginResultDto> StaffLoginAsync(PortalLoginDto dto)
    {
        var staff = await _staffRepo.Query().IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Username == dto.Username && s.SchoolId == dto.SchoolId && !s.IsDeleted);

        if (staff == null || staff.Password != dto.Password)
            return new PortalLoginResultDto { Error = "InvalidCredentials" };

        var token = GeneratePortalToken(staff.Id, staff.FullName, "Staff", staff.SchoolId, staff.BranchId);
        return new PortalLoginResultDto
        {
            Succeeded = true, Token = token, FullName = staff.FullName,
            UserType = "Staff", PersonId = staff.Id,
            SchoolId = staff.SchoolId, BranchId = staff.BranchId
        };
    }

    private string GeneratePortalToken(int personId, string fullName, string userType, int schoolId, int? branchId)
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

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"]!)),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
