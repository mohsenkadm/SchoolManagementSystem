using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers
{
    [ApiController]
    [Route("api/{schoolId:int}/School")]
    [Authorize]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolService _service;
        public SchoolController(ISchoolService service) => _service = service;

        // جلب جميع فروع المدرسة
        [HttpGet]
        public async Task<ActionResult<List<SchoolDto>>> GetBySchoolIdAsync(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));
    }
}
