using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة الإعلانات
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/announcements")]
[Authorize]
public class AnnouncementsApiController : ControllerBase
{
    private readonly IAnnouncementService _service;
    public AnnouncementsApiController(IAnnouncementService service) => _service = service;

    // جلب جميع إعلانات المدرسة
    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetAll(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetAllAsync(schoolId, branchId));

    // جلب إعلان بالمعرف
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> GetById(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null || item.SchoolId != schoolId) return NotFound();
        return Ok(item);
    }

    // إنشاء إعلان جديد
    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> Create(int schoolId, [FromBody] AnnouncementDto dto)
    {
        dto.SchoolId = schoolId;
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { schoolId, id = created.Id }, created);
    }

    // تعديل إعلان
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> Update(int schoolId, int id, [FromBody] AnnouncementDto dto)
    {
        dto.Id = id;
        dto.SchoolId = schoolId;
        var updated = await _service.UpdateAsync(dto);
        return Ok(updated);
    }

    // حذف إعلان
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int schoolId, int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null || item.SchoolId != schoolId) return NotFound();
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
