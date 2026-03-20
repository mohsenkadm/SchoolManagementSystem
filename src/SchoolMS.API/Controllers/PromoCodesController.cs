using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة أكواد الخصم
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/promo-codes")]
[Authorize]
public class PromoCodesController : ControllerBase
{
    private readonly IPromoCodeService _service;
    public PromoCodesController(IPromoCodeService service) => _service = service;

    // جلب جميع أكواد الخصم للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<PromoCodeDto>>> GetAll(int schoolId)
        => Ok(await _service.GetBySchoolIdAsync(schoolId));

    // جلب كود خصم بالنص
    [HttpGet("code/{code}")]
    public async Task<ActionResult<PromoCodeDto>> GetByCode(int schoolId, string code)
    {
        var item = await _service.GetByCodeAsync(code);
        return item == null ? NotFound() : Ok(item);
    }
                            
}
