using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Enums;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الرواتب
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/salaries")]
[Authorize]
public class SalariesApiController : ControllerBase
{
    private readonly ISalaryService _service;
    public SalariesApiController(ISalaryService service) => _service = service;

    // جلب راتب الشخص المسجل دخوله
    [HttpGet("my-salary")]
    public async Task<ActionResult<SalarySetupDto>> GetMySalary(int schoolId)
    {
        var userType = User.FindFirst("UserType")?.Value;
        var personId = int.Parse(User.FindFirst("PersonId")?.Value ?? throw new UnauthorizedAccessException());

        PersonType? personType = userType switch
        {
            "Teacher" => PersonType.Teacher,
            "Staff" => PersonType.Staff,
            _ => null
        };
        if (personType == null) return Forbid();

        var result = await _service.GetByPersonAsync(personId, personType.Value, schoolId);
        return result == null ? NotFound() : Ok(result);
    }
}
