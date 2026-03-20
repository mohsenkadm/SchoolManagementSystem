using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الأقساط والمدفوعات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/installments")]
[Authorize]
public class InstallmentsApiController : ControllerBase
{
    private readonly IInstallmentService _service;
    public InstallmentsApiController(IInstallmentService service) => _service = service;

    private int GetPersonIdFromToken() =>
        int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());

    private string GetUserTypeFromToken() =>
        User.FindFirst("UserType")?.Value ?? throw new UnauthorizedAccessException();

    // أقساط الطالب - يتم جلب معرف الطالب من التوكن
    [HttpGet("student")]
    public async Task<ActionResult<List<FeeInstallmentDto>>> GetStudentInstallments(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        return Ok(await _service.GetByStudentIdAsync(studentId, schoolId));
    }

    // أقساط أبناء ولي الأمر - يتم جلب معرف ولي الأمر من التوكن
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<FeeInstallmentDto>>> GetParentChildrenInstallments(int schoolId)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        return Ok(await _service.GetByParentChildrenAsync(parentId, schoolId));
    }
}
