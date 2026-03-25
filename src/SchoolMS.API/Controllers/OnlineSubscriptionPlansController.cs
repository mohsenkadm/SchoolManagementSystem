using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة خطط الاشتراك الإلكترونية
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/subscription-plans")]
[Authorize]
public class OnlineSubscriptionPlansController : ControllerBase
{
    private readonly IOnlineSubscriptionPlanService _service;
    public OnlineSubscriptionPlansController(IOnlineSubscriptionPlanService service) => _service = service;

    // جلب جميع خطط الاشتراك للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<OnlineSubscriptionPlanDto>>> GetAll(int schoolId, [FromQuery] string? search = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, search));

    // جلب خطط الاشتراك المتاحة للطلاب فقط (نوع Course أو LiveStream)
    [HttpGet("student")]
    public async Task<ActionResult<List<OnlineSubscriptionPlanDto>>> GetForStudent(int schoolId,
        [FromQuery] string? search = null)
    {
        var userType = User.FindFirst("UserType")?.Value;
        if (userType != "Student") return Forbid();
        var items = await _service.GetBySchoolIdAsync(schoolId, search);
        return Ok(items);
    }

}
