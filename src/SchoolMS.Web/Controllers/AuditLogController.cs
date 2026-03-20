using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class AuditLogController : Controller
{
    private readonly IAuditLogService _service;
    public AuditLogController(IAuditLogService service) => _service = service;

    [HasPermission("Users", "View")]
    public IActionResult Index() { ViewData["Title"] = "Audit Log"; return View(); }

    [HttpPost, HasPermission("AuditLog", "View"), ValidateAntiForgeryToken]
    public async Task<IActionResult> GetData([FromBody] DataTableRequest request) => Ok(await _service.GetDataTableAsync(request));
}
