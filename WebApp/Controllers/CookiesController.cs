using System.Text.Json;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class CookiesController : Controller
    {

        [HttpPost]
        public IActionResult SetCookies([FromBody] CookieConsent consent)
        {
            Response.Cookies.Append("SessionCookie", "Essential", new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(90),
                SameSite = SameSiteMode.Lax,
                Secure = true
            });

            if (consent == null)
                return BadRequest("Consent data is required.");

            if (consent.Functional)
            {
                Response.Cookies.Append("FunctionalCookie", "Non-Essential", new CookieOptions
                {
                    IsEssential = false,
                    Expires = DateTimeOffset.UtcNow.AddDays(90),
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                });
            }
            else
            {
                Response.Cookies.Delete("FunctionalCookie", new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
            }

            if (consent.Analytics)
            {
                Response.Cookies.Append("AnalyticsCookie", "Non-Essential", new CookieOptions
                {
                    IsEssential = false,
                    Expires = DateTimeOffset.UtcNow.AddDays(90),
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                });
            }
            else
            {
                Response.Cookies.Delete("AnalyticsCookie", new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
            }

            if (consent.Marketing)
            {
                Response.Cookies.Append("MarketingCookie", "Non-Essential", new CookieOptions
                {
                    IsEssential = false,
                    Expires = DateTimeOffset.UtcNow.AddDays(90),
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                });
            }
            else
            {
                Response.Cookies.Delete("MarketingCookie", new CookieOptions
                {
                    Path = "/",
                    SameSite = SameSiteMode.Lax
                });
            }

            Response.Cookies.Append("cookieConsent", JsonSerializer.Serialize(consent), new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(90),
                SameSite = SameSiteMode.Lax,
                Path = "/",
            });

            return Ok();
        }
    }
}
