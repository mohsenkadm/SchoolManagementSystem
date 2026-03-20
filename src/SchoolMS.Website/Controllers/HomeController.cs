using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Website.Models;

namespace SchoolMS.Website.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRegistrationRequestService _registrationService;
    private readonly IPlatformService _platformService;

    public HomeController(ILogger<HomeController> logger, IRegistrationRequestService registrationService, IPlatformService platformService)
    {
        _logger = logger;
        _registrationService = registrationService;
        _platformService = platformService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Plans = await _platformService.GetAllPlansAsync();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([FromBody] CreateRegistrationRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _registrationService.CreateAsync(dto, ipAddress);
        return Ok(result);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
