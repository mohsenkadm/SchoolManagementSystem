using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة طلبات الإجازات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/leaves")]
[Authorize]
public class LeavesApiController : ControllerBase
{
    private readonly ILeaveService _service;
    private readonly IOneSignalNotificationService _pushService;
    public LeavesApiController(ILeaveService service, IOneSignalNotificationService pushService) { _service = service; _pushService = pushService; }

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());

    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // جلب جميع طلبات الإجازات للمدرسة مع فلاتر
    [HttpGet]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetAll(int schoolId,
        [FromQuery] string? status = null, [FromQuery] string? personType = null)
    {
        var items = await _service.GetBySchoolIdAsync(schoolId);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, true, out var ls))
            items = items.Where(l => l.Status == ls).ToList();
        if (!string.IsNullOrEmpty(personType) && Enum.TryParse<PersonType>(personType, true, out var pt))
            items = items.Where(l => l.PersonType == pt).ToList();
        return Ok(items);
    }

    // جلب إجازات الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetStudentLeaves(int schoolId,
        [FromQuery] string? status = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var items = await _service.GetByPersonAsync(studentId, PersonType.Student, schoolId);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, true, out var ls))
            items = items.Where(l => l.Status == ls).ToList();
        return Ok(items);
    }

    // جلب إجازات أبناء ولي الأمر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetParentChildrenLeaves(int schoolId,
        [FromQuery] string? status = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        var items = await _service.GetByParentChildrenAsync(parentId, schoolId);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveStatus>(status, true, out var ls))
            items = items.Where(l => l.Status == ls).ToList();
        return Ok(items);
    }

    // إنشاء طلب إجازة جديد
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(int schoolId, [FromBody] LeaveRequestDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Leave Request", "A new leave request has been submitted", new[] { "Staff" }, schoolId); return Ok(r); }
}
