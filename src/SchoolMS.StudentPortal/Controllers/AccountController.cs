using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.StudentPortal.Controllers;

public class AccountController : Controller
{
    private readonly SchoolDbContext _context;
    private readonly IPortalAuthService _authService;

    public AccountController(SchoolDbContext context, IPortalAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    /// <summary>
    /// Gets or creates a stable device ID stored in a long-lived cookie so that
    /// restarting the server / refreshing doesn't generate a new device fingerprint.
    /// </summary>
    private string GetOrCreateDeviceId()
    {
        const string cookieName = "SP_DeviceId";
        if (Request.Cookies.TryGetValue(cookieName, out var existing) && !string.IsNullOrWhiteSpace(existing))
            return existing;

        var newId = $"WEB-{Guid.NewGuid():N}";
        Response.Cookies.Append(cookieName, newId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            Expires = DateTimeOffset.UtcNow.AddYears(2),
            SameSite = SameSiteMode.Lax
        });
        return newId;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? slug)
    {
        // Check session first, then fall back to persistent auth cookie
        if (HttpContext.Session.GetInt32("StudentId") != null)
            return RedirectToAction("Index", "Home");

        if (User.Identity?.IsAuthenticated == true)
        {
            var idClaim = User.FindFirst("StudentId")?.Value;
            if (int.TryParse(idClaim, out var cookieStudentId))
            {
                // Restore session from cookie claims
                var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
                var nameClaim = User.FindFirst("StudentName")?.Value;
                var slugClaim = User.FindFirst("SchoolSlug")?.Value;
                var deviceClaim = User.FindFirst("DeviceId")?.Value;
                var branchClaim = User.FindFirst("BranchId")?.Value;

                if (int.TryParse(schoolIdClaim, out var schoolId))
                {
                    HttpContext.Session.SetInt32("StudentId", cookieStudentId);
                    HttpContext.Session.SetInt32("SchoolId", schoolId);
                    HttpContext.Session.SetString("StudentName", nameClaim ?? "");
                    HttpContext.Session.SetString("SchoolSlug", slugClaim ?? "");
                    HttpContext.Session.SetString("DeviceId", deviceClaim ?? "");
                    if (int.TryParse(branchClaim, out var branchId))
                        HttpContext.Session.SetInt32("BranchId", branchId);
                }

                return RedirectToAction("Index", "Home");
            }
        }

        if (!string.IsNullOrEmpty(slug))
        {
            var school = await _context.Schools.IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Slug == slug && !s.IsDeleted);
            if (school != null && school.IsActive)
            {
                ViewBag.SchoolSlug = slug;
                ViewBag.SchoolName = school.Name;
                ViewBag.SchoolLogo = school.Logo;
                ViewBag.SchoolId = school.Id;
            }
            else
            {
                ViewBag.Error = school == null ? "المدرسة غير موجودة" : "المدرسة معطّلة حالياً";
            }
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? slug, string username, string password)
    {
        if (string.IsNullOrEmpty(slug))
        {
            ViewBag.Error = "يجب تحديد المدرسة في الرابط";
            return View();
        }

        var school = await _context.Schools.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Slug == slug && !s.IsDeleted);

        if (school == null || !school.IsActive)
        {
            ViewBag.Error = "المدرسة غير موجودة أو معطّلة";
            ViewBag.SchoolSlug = slug;
            return View();
        }

        ViewBag.SchoolSlug = slug;
        ViewBag.SchoolName = school.Name;
        ViewBag.SchoolLogo = school.Logo;
        ViewBag.SchoolId = school.Id;

        var deviceId = GetOrCreateDeviceId();

        var result = await _authService.StudentLoginAsync(new Application.DTOs.PortalLoginDto
        {
            Username = username,
            Password = password,
            SchoolId = school.Id,
            DeviceId = deviceId
        });

        if (!result.Succeeded)
        {
            ViewBag.Error = result.Error switch
            {
                "InvalidCredentials" => "اسم المستخدم أو كلمة المرور غير صحيحة",
                "DeviceAlreadyActive" => result.ErrorMessage ?? "حسابك مسجّل على جهاز آخر. يُسمح بجهاز واحد فقط.",
                "DeviceIdRequired" => "خطأ في المصادقة",
                _ => "حدث خطأ غير متوقع"
            };
            return View();
        }

        // Set session
        HttpContext.Session.SetInt32("StudentId", result.PersonId);
        HttpContext.Session.SetInt32("SchoolId", result.SchoolId);
        HttpContext.Session.SetString("StudentName", result.FullName ?? "");
        HttpContext.Session.SetString("SchoolSlug", slug);
        HttpContext.Session.SetString("DeviceId", deviceId);
        if (result.BranchId.HasValue)
            HttpContext.Session.SetInt32("BranchId", result.BranchId.Value);

        // Issue persistent auth cookie so login survives server restarts
        await SignInWithCookieAsync(result.PersonId, result.SchoolId, result.FullName ?? "",
            slug, deviceId, result.BranchId);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var studentId = HttpContext.Session.GetInt32("StudentId");
        var slug = HttpContext.Session.GetString("SchoolSlug");
        if (studentId.HasValue)
            await _authService.LogoutStudentDeviceAsync(studentId.Value);

        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", new { slug });
    }

    // ===================== REGISTER =====================

    [HttpGet]
    public async Task<IActionResult> Register(string? slug)
    {
        if (HttpContext.Session.GetInt32("StudentId") != null)
            return RedirectToAction("Index", "Home");

        await PopulateSchoolViewBag(slug);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string? slug, string firstName, string lastName,
        string username, string email, string phone, string password, string confirmPassword)
    {
        var school = await GetSchoolBySlug(slug);
        await PopulateSchoolViewBag(slug);

        if (school == null)
        {
            ViewBag.Error = "يجب تحديد المدرسة في الرابط";
            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.Error = "كلمة المرور وتأكيدها غير متطابقتين";
            return View();
        }

        // Check if username already exists
        var existingUser = await _context.Students.IgnoreQueryFilters()
            .AnyAsync(s => s.Username == username && s.SchoolId == school.Id && !s.IsDeleted);
        if (existingUser)
        {
            ViewBag.Error = "اسم المستخدم مُستخدم بالفعل";
            return View();
        }

        // Check if phone already exists
        var existingPhone = await _context.Students.IgnoreQueryFilters()
            .AnyAsync(s => s.Phone == phone && s.SchoolId == school.Id && !s.IsDeleted);
        if (existingPhone)
        {
            ViewBag.Error = "رقم الهاتف مسجّل بالفعل";
            return View();
        }

        // Generate OTP and store in session
        var otp = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("RegisterOtp", otp);
        HttpContext.Session.SetString("RegisterData", System.Text.Json.JsonSerializer.Serialize(new
        {
            FirstName = firstName,
            LastName = lastName,
            Username = username,
            Email = email,
            Phone = phone,
            Password = password,
            SchoolId = school.Id
        }));

        // TODO: Integrate SMS/email provider to send OTP to phone
        // For now, store OTP in TempData so it can be tested
        TempData["DebugOtp"] = otp;

        return RedirectToAction("VerifyOtp", new { slug, context = "register" });
    }

    // ===================== FORGOT PASSWORD =====================

    [HttpGet]
    public async Task<IActionResult> ForgotPassword(string? slug)
    {
        await PopulateSchoolViewBag(slug);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(string? slug, string phone)
    {
        var school = await GetSchoolBySlug(slug);
        await PopulateSchoolViewBag(slug);

        if (school == null)
        {
            ViewBag.Error = "يجب تحديد المدرسة في الرابط";
            return View();
        }

        var student = await _context.Students.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Phone == phone && s.SchoolId == school.Id && !s.IsDeleted);

        if (student == null)
        {
            ViewBag.Error = "لم يتم العثور على حساب بهذا الرقم";
            return View();
        }

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();
        HttpContext.Session.SetString("ResetOtp", otp);
        HttpContext.Session.SetInt32("ResetStudentId", student.Id);
        HttpContext.Session.SetString("ResetSlug", slug ?? "");

        // TODO: Integrate SMS provider to send OTP to phone
        TempData["DebugOtp"] = otp;

        return RedirectToAction("VerifyOtp", new { slug, context = "reset" });
    }

    // ===================== VERIFY OTP =====================

    [HttpGet]
    public async Task<IActionResult> VerifyOtp(string? slug, string? context)
    {
        await PopulateSchoolViewBag(slug);
        ViewBag.OtpContext = context ?? "register";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(string? slug, string? context, string otp)
    {
        await PopulateSchoolViewBag(slug);
        ViewBag.OtpContext = context ?? "register";

        if (string.IsNullOrWhiteSpace(otp) || otp.Length < 6)
        {
            ViewBag.Error = "يرجى إدخال الرمز كاملاً";
            return View();
        }

        if (context == "register")
        {
            var savedOtp = HttpContext.Session.GetString("RegisterOtp");
            if (savedOtp == null || savedOtp != otp)
            {
                ViewBag.Error = "الرمز غير صحيح";
                return View();
            }

            var dataJson = HttpContext.Session.GetString("RegisterData");
            if (string.IsNullOrEmpty(dataJson))
            {
                ViewBag.Error = "انتهت صلاحية البيانات، يرجى إعادة التسجيل";
                return View();
            }

            var data = System.Text.Json.JsonSerializer.Deserialize<RegisterSessionData>(dataJson);
            if (data == null)
            {
                ViewBag.Error = "خطأ في البيانات";
                return View();
            }

            // Get default branch, classroom, academic year for the school
            var branch = await _context.Branches.IgnoreQueryFilters()
                .FirstOrDefaultAsync(b => b.SchoolId == data.SchoolId && !b.IsDeleted);
            var classRoom = await _context.ClassRooms.IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.SchoolId == data.SchoolId && !c.IsDeleted);
            var academicYear = await _context.AcademicYears.IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.SchoolId == data.SchoolId && !a.IsDeleted);

            if (branch == null || classRoom == null || academicYear == null)
            {
                ViewBag.Error = "المدرسة غير مهيّأة بالكامل بعد. تواصل مع الإدارة.";
                return View();
            }

            var student = new Student
            {
                FullName = $"{data.FirstName} {data.LastName}",
                Username = data.Username,
                Email = data.Email,
                Phone = data.Phone,
                Password = data.Password,
                SchoolId = data.SchoolId,
                BranchId = branch.Id,
                ClassRoomId = classRoom.Id,
                AcademicYearId = academicYear.Id,
                DateOfBirth = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SelfRegister"
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Clear registration session data
            HttpContext.Session.Remove("RegisterOtp");
            HttpContext.Session.Remove("RegisterData");

            // Auto-login
            var deviceId = GetOrCreateDeviceId();
            student.ActiveDeviceId = deviceId;
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("StudentId", student.Id);
            HttpContext.Session.SetInt32("SchoolId", student.SchoolId);
            HttpContext.Session.SetString("StudentName", student.FullName);
            HttpContext.Session.SetString("SchoolSlug", slug ?? "");
            HttpContext.Session.SetString("DeviceId", deviceId);
            HttpContext.Session.SetInt32("BranchId", student.BranchId);

            await SignInWithCookieAsync(student.Id, student.SchoolId, student.FullName,
                slug ?? "", deviceId, student.BranchId);

            return RedirectToAction("Index", "Home");
        }
        else // reset
        {
            var savedOtp = HttpContext.Session.GetString("ResetOtp");
            if (savedOtp == null || savedOtp != otp)
            {
                ViewBag.Error = "الرمز غير صحيح";
                return View();
            }

            // OTP verified — allow password reset
            HttpContext.Session.SetString("ResetVerified", "true");
            return RedirectToAction("ResetPassword", new { slug });
        }
    }

    // ===================== RESET PASSWORD =====================

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string? slug)
    {
        await PopulateSchoolViewBag(slug);

        var verified = HttpContext.Session.GetString("ResetVerified");
        if (verified != "true")
            return RedirectToAction("ForgotPassword", new { slug });

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string? slug, string newPassword, string confirmPassword)
    {
        await PopulateSchoolViewBag(slug);

        var verified = HttpContext.Session.GetString("ResetVerified");
        if (verified != "true")
            return RedirectToAction("ForgotPassword", new { slug });

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "كلمة المرور وتأكيدها غير متطابقتين";
            return View();
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            ViewBag.Error = "كلمة المرور يجب أن تكون 6 أحرف على الأقل";
            return View();
        }

        var studentId = HttpContext.Session.GetInt32("ResetStudentId");
        if (!studentId.HasValue)
        {
            ViewBag.Error = "انتهت صلاحية الجلسة، يرجى إعادة المحاولة";
            return View();
        }

        var student = await _context.Students.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == studentId.Value && !s.IsDeleted);
        if (student == null)
        {
            ViewBag.Error = "لم يتم العثور على الحساب";
            return View();
        }

        student.Password = newPassword;
        student.ActiveDeviceId = null; // Clear device so they can log in fresh
        await _context.SaveChangesAsync();

        // Clear reset session
        HttpContext.Session.Remove("ResetOtp");
        HttpContext.Session.Remove("ResetStudentId");
        HttpContext.Session.Remove("ResetVerified");
        HttpContext.Session.Remove("ResetSlug");

        return RedirectToAction("Login", new { slug });
    }

    // ===================== HELPERS =====================

    private async Task<School?> GetSchoolBySlug(string? slug)
    {
        if (string.IsNullOrEmpty(slug)) return null;
        return await _context.Schools.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Slug == slug && !s.IsDeleted && s.IsActive);
    }

    private async Task PopulateSchoolViewBag(string? slug)
    {
        ViewBag.SchoolSlug = slug;
        if (!string.IsNullOrEmpty(slug))
        {
            var school = await GetSchoolBySlug(slug);
            if (school != null)
            {
                ViewBag.SchoolName = school.Name;
                ViewBag.SchoolLogo = school.Logo;
                ViewBag.SchoolId = school.Id;
            }
        }
    }

    private async Task SignInWithCookieAsync(int studentId, int schoolId, string studentName,
        string slug, string deviceId, int? branchId)
    {
        var claims = new List<Claim>
        {
            new("StudentId", studentId.ToString()),
            new("SchoolId", schoolId.ToString()),
            new("StudentName", studentName),
            new("SchoolSlug", slug),
            new("DeviceId", deviceId)
        };
        if (branchId.HasValue)
            claims.Add(new Claim("BranchId", branchId.Value.ToString()));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });
    }

    private class RegisterSessionData
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Password { get; set; } = "";
        public int SchoolId { get; set; }
    }
}
