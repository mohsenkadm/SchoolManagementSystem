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

    // جلب إجازات الطالب
    [HttpGet("student")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetStudentLeaves(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        return Ok(await _service.GetByPersonAsync(studentId, PersonType.Student, schoolId));
    }

    // جلب إجازات أبناء ولي الأمر
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetParentChildrenLeaves(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        return Ok(await _service.GetByParentChildrenAsync(parentId, schoolId));
    }

    // إنشاء طلب إجازة جديد
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> Create(int schoolId, [FromBody] LeaveRequestDto dto)
    { var r = await _service.CreateAsync(dto); await _pushService.SendToPersonTypesAsync("New Leave Request", "A new leave request has been submitted", new[] { "Staff" }, schoolId); return Ok(r); }
}
