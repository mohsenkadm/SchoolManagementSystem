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

    // جلب جميع أقساط المدرسة مع فلاتر اختيارية
    [HttpGet]
    public async Task<ActionResult<List<FeeInstallmentDto>>> GetAll(int schoolId,
        [FromQuery] int? branchId = null, [FromQuery] int? academicYearId = null,
        [FromQuery] int? studentId = null)
    {
        var items = await _service.GetBySchoolIdAsync(schoolId, branchId);
        if (academicYearId.HasValue) items = items.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
        if (studentId.HasValue) items = items.Where(i => i.StudentId == studentId.Value).ToList();
        return Ok(items);
    }

    // أقساط الطالب - يتم جلب معرف الطالب من التوكن
    [HttpGet("student")]
    public async Task<ActionResult<List<FeeInstallmentDto>>> GetStudentInstallments(int schoolId,
        [FromQuery] int? academicYearId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Student") return Forbid();
        var studentId = GetPersonIdFromToken();
        var items = await _service.GetByStudentIdAsync(studentId, schoolId);
        if (academicYearId.HasValue) items = items.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
        return Ok(items);
    }

    // أقساط أبناء ولي الأمر - يتم جلب معرف ولي الأمر من التوكن
    [HttpGet("parent/children")]
    public async Task<ActionResult<List<FeeInstallmentDto>>> GetParentChildrenInstallments(int schoolId,
        [FromQuery] int? academicYearId = null)
    {
        var userType = GetUserTypeFromToken();
        if (userType != "Parent") return Forbid();
        var parentId = GetPersonIdFromToken();
        var items = await _service.GetByParentChildrenAsync(parentId, schoolId);
        if (academicYearId.HasValue) items = items.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
        return Ok(items);
    }
}
