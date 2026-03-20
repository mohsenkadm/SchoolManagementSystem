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

    // جلب جميع كتب المدرسة
    [HttpGet("books")]
    public async Task<ActionResult<List<LibraryBookDto>>> GetAllBooks(int schoolId, [FromQuery] int? branchId = null)
        => Ok(await _service.GetBooksBySchoolIdAsync(schoolId, branchId));

    // جلب كتاب بالمعرف
    [HttpGet("books/{id}")]
    public async Task<ActionResult<LibraryBookDto>> GetBook(int schoolId, int id)
    {
        var item = await _service.GetBookByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }
                                 
}
