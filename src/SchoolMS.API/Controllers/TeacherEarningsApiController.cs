using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة أرباح المدرسين من الكورسات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/teacher-earnings")]
[Authorize]
public class TeacherEarningsApiController : ControllerBase
{
    private readonly ITeacherEarningService _service;
    private readonly IRepository<Teacher> _teacherRepo;

    public TeacherEarningsApiController(ITeacherEarningService service, IRepository<Teacher> teacherRepo)
    {
        _service = service;
        _teacherRepo = teacherRepo;
    }

    /// <summary>
    /// جلب أرباح المدرس المسجل دخوله
    /// </summary>
    [HttpGet("my-earnings")]
    public async Task<ActionResult<List<TeacherEarningDto>>> GetMyEarnings(int schoolId)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Teacher") return Forbid();

        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == username);
        if (teacher == null) return NotFound(new { error = "Teacher profile not found." });

        var earnings = await _service.GetByTeacherIdAsync(teacher.Id, schoolId);
        return Ok(earnings);
    }

    /// <summary>
    /// جلب ملخص أرباح المدرس المسجل دخوله
    /// </summary>
    [HttpGet("my-summary")]
    public async Task<ActionResult<TeacherEarningSummaryDto>> GetMySummary(int schoolId)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Teacher") return Forbid();

        var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.Username == username);
        if (teacher == null) return NotFound(new { error = "Teacher profile not found." });

        var summary = await _service.GetTeacherSummaryAsync(teacher.Id, schoolId);
        return summary == null ? NotFound() : Ok(summary);
    }

    /// <summary>
    /// جلب جميع أرباح المدرسين (للمشرف)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TeacherEarningDto>>> GetAll(int schoolId)
    {
        var earnings = await _service.GetBySchoolIdAsync(schoolId);
        return Ok(earnings);
    }

    /// <summary>
    /// جلب أرباح مدرس معين
    /// </summary>
    [HttpGet("teacher/{teacherId:int}")]
    public async Task<ActionResult<List<TeacherEarningDto>>> GetByTeacher(int schoolId, int teacherId)
    {
        var earnings = await _service.GetByTeacherIdAsync(teacherId, schoolId);
        return Ok(earnings);
    }

    /// <summary>
    /// جلب ملخص أرباح جميع المدرسين
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<List<TeacherEarningSummaryDto>>> GetSummary(int schoolId)
    {
        var summaries = await _service.GetSummaryBySchoolIdAsync(schoolId);
        return Ok(summaries);
    }
}
