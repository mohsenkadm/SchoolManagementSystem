using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using SchoolMS.Application;
using SchoolMS.Application.Settings;
using SchoolMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// OneSignal settings
builder.Services.Configure<OneSignalSettings>(builder.Configuration.GetSection("OneSignal"));

// Localization
builder.Services.AddLocalization();

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Supported cultures
var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("ar");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

// Cookie authentication for persistent login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = "SP_Auth";
    });

// Session for student auth
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Middleware: restore session from persistent cookie if session is empty but cookie is valid
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true
        && context.Session.GetInt32("StudentId") == null)
    {
        var idClaim = context.User.FindFirst("StudentId")?.Value;
        var schoolIdClaim = context.User.FindFirst("SchoolId")?.Value;
        var nameClaim = context.User.FindFirst("StudentName")?.Value;
        var slugClaim = context.User.FindFirst("SchoolSlug")?.Value;
        var deviceClaim = context.User.FindFirst("DeviceId")?.Value;
        var branchClaim = context.User.FindFirst("BranchId")?.Value;

        if (int.TryParse(idClaim, out var studentId) && int.TryParse(schoolIdClaim, out var schoolId))
        {
            context.Session.SetInt32("StudentId", studentId);
            context.Session.SetInt32("SchoolId", schoolId);
            context.Session.SetString("StudentName", nameClaim ?? "");
            context.Session.SetString("SchoolSlug", slugClaim ?? "");
            context.Session.SetString("DeviceId", deviceClaim ?? "");
            if (int.TryParse(branchClaim, out var branchId))
                context.Session.SetInt32("BranchId", branchId);
        }
    }
    await next();
});

app.MapControllerRoute(
    name: "school-portal",
    pattern: "school/{slug}",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
