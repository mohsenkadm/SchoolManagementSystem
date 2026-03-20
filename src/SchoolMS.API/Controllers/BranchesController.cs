using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الفروع
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/branches")]
[Authorize]
public class BranchesController : ControllerBase
{
    private readonly IBranchService _service;
    public BranchesController(IBranchService service) => _service = service;

    // جلب جميع فروع المدرسة
    [HttpGet]
    public async Task<ActionResult<List<BranchDto>>> GetAll(int schoolId) => Ok(await _service.GetBySchoolIdAsync(schoolId));

}
