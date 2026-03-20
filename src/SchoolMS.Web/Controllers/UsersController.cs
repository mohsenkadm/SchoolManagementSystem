using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Web.Filters;

namespace SchoolMS.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserManagementService _service;
    private readonly ISubscriptionLimitService _limitService;
    private readonly IOneSignalNotificationService _pushService;

    public UsersController(IUserManagementService service, ISubscriptionLimitService limitService, IOneSignalNotificationService pushService)
    {
        _service = service;
        _limitService = limitService;
        _pushService = pushService;
    }

    [HasPermission("Users", "View")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "User Management";
        ViewBag.Roles = await _service.GetAllRolesAsync();
        ViewBag.Permissions = await _service.GetAllPermissionsAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetData([FromBody] DataTableRequest request) => Ok(await _service.GetDataTableAsync(request));

    [HttpGet]
    public async Task<IActionResult> Get(string id) => Json(await _service.GetUserByIdAsync(id));

    [HttpGet]
    public async Task<IActionResult> GetPermissions() => Json(await _service.GetAllPermissionsAsync());

    [HttpPost, HasPermission("Users", "Add"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var schoolClaim = User.FindFirst("SchoolId");
        if (schoolClaim != null && int.TryParse(schoolClaim.Value, out var schoolId))
        {
            var (allowed, error) = await _limitService.CanAddUserAsync(schoolId);
            if (!allowed) return BadRequest(error);
        }
        var result = await _service.CreateUserAsync(dto);
        if (schoolClaim != null && int.TryParse(schoolClaim.Value, out var sid))
            await _pushService.SendToPersonTypesAsync("New User Created",
                $"{dto.FullName} has been added as {dto.UserType}",
                new[] { "Staff" }, sid);
        return Ok(result);
    }

    [HttpPut, HasPermission("Users", "Edit"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto) => Ok(await _service.UpdateUserAsync(dto));

    [HttpDelete("{id}"), HasPermission("Users", "Delete")]
    public async Task<IActionResult> Delete(string id) { await _service.DeleteUserAsync(id); return Ok(); }
}
