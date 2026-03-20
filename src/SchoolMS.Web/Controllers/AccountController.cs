using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly SchoolDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        SchoolDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(string? slug, string? school, string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["SchoolSlug"] = slug ?? school;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        // Find school by slug if provided
        School? school = null;
        if (!string.IsNullOrEmpty(model.SchoolSlug))
        {
            school = await _context.Schools.IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Slug == model.SchoolSlug && !s.IsDeleted);
            if (school == null)
            {
                ModelState.AddModelError("", "School not found.");
                return View(model);
            }
            if (!school.IsActive)
            {
                ModelState.AddModelError("", "School is deactivated. Contact the administrator.");
                return View(model);
            }

            // Check subscription expiry
            var activeSub = await _context.SchoolSubscriptions.IgnoreQueryFilters()
                .Where(s => s.SchoolId == school.Id && s.IsActive && !s.IsDeleted)
                .OrderByDescending(s => s.ActivatedAt)
                .FirstOrDefaultAsync();
            if (activeSub != null && activeSub.ExpiryDate < DateTime.UtcNow)
            {
                ModelState.AddModelError("", "School subscription has expired. Contact the administrator.");
                return View(model);
            }
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || user.IsDeleted)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        if (school != null && user.SchoolId != school.Id)
        {
            ModelState.AddModelError("", "User does not belong to this school.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            // Remove old custom claims to avoid duplicates
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var oldCustom = existingClaims.Where(c => c.Type is "SchoolId" or "FullName" or "UserType" or "BranchId").ToList();
            if (oldCustom.Count > 0)
                await _userManager.RemoveClaimsAsync(user, oldCustom);

            // Check if user is SuperAdmin
            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");

            // Add custom claims
            var claims = new List<Claim>
            {
                new("FullName", user.FullName),
                new("UserType", user.UserType.ToString())
            };

            // SuperAdmin: only set SchoolId if logging in via a specific school slug
            // Otherwise leave it out so the global query filter shows ALL schools
            if (isSuperAdmin && school == null)
            {
                // No SchoolId claim → TenantProvider returns null → no school filter
            }
            else
            {
                claims.Add(new Claim("SchoolId", user.SchoolId.ToString()));
            }

            if (user.BranchId.HasValue)
                claims.Add(new Claim("BranchId", user.BranchId.Value.ToString()));

            await _userManager.AddClaimsAsync(user, claims);
            await _signInManager.RefreshSignInAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid email or password.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
