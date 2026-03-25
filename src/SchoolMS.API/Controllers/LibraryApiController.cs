using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;

namespace SchoolMS.API.Controllers;

/// <summary>
/// إدارة المكتبة
/// </summary>
[ApiController]
[Route("api/{schoolId:int}/library")]
[Authorize]
public class LibraryApiController : ControllerBase
{
    private readonly ILibraryService _service;
    public LibraryApiController(ILibraryService service) => _service = service;

    // ========== الكتب ==========

    // جلب جميع كتب المدرسة مع فلاتر
    [HttpGet("books")]
    public async Task<ActionResult<List<LibraryBookDto>>> GetAllBooks(int schoolId,
        [FromQuery] int? branchId = null, [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        var items = await _service.GetBooksBySchoolIdAsync(schoolId, branchId);
        if (!string.IsNullOrEmpty(search))
            items = items.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (b.Author != null && b.Author.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
        if (!string.IsNullOrEmpty(category))
            items = items.Where(b => b.Category != null && b.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        return Ok(items);
    }

    // جلب كتاب بالمعرف
    [HttpGet("books/{id}")]
    public async Task<ActionResult<LibraryBookDto>> GetBook(int schoolId, int id)
    {
        var item = await _service.GetBookByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

}
