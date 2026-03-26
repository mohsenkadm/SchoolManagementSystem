using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace SchoolMS.StudentPortal.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult Set(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

        return LocalRedirect(returnUrl);
    }
}
