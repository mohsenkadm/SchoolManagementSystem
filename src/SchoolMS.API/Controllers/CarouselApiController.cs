using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

[ApiController]
[Route("api/{schoolId:int}/carousel")]
[Authorize]
public class CarouselApiController : ControllerBase
{
    private readonly ICarouselService _service;
    public CarouselApiController(ICarouselService service) => _service = service;

    // جلب جميع صور الكاروسيل للمدرسة
    [HttpGet]
    public async Task<ActionResult<List<CarouselImageDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetBySchoolIdAsync(schoolId, branchId));

}
