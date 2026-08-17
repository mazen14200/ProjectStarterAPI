using Domain.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication.Attributes;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        // SECURITY FIX: Add cache control for sensitive pages
        // Prevents caching of pages containing user data
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View();
        }

        // SECURITY FIX: Add cache control for privacy page
        // Privacy pages contain sensitive user information
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Privacy()
        {
            return View();
        }

        // Used For Change Language [ Ar - En ]
        [IgnoreAction]
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ChangeLanguage(string lang)
        {
            HttpContext.Session.SetString("CurrentCulture", lang);

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );

            // Redirect to previous page (Referer)
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer ?? "/");
        }
        // SECURITY FIX: Add ValidateAntiForgeryToken to POST actions
        // Protects against CSRF attacks by validating anti-forgery tokens
        // Example: [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult SomeAction() { }
        [IgnoreAction]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // Error 500
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionHandlerPathFeature != null)
            {
                ViewBag.Path = exceptionHandlerPathFeature.Path;
                ViewBag.Message = exceptionHandlerPathFeature.Error.Message;
            }

            return View("Error500"); // Error 500
        }

        [IgnoreAction]
        //[NoLogging]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = Resource1.pageNotFound;  //"page Not Found"
                    return View("Error404");
                case 403:
                    ViewBag.ErrorMessage = Resource1.YouNotAuthorizedAccessPage;//"You are not authorized to access this page"
                    return View("AccessDeniedError403");
                case 401:
                    ViewBag.ErrorMessage = Resource1.YouNotAuthorizedAccessPage; // Not Loggined in now UN Authorization
                    return View("AccessDeniedError403");
                case 500:
                    return View("Error500"); // Server error
                case 503:
                    return View("Error503"); // The server is not available now
                default:
                    ViewBag.ErrorMessage = Resource1.AnUnexpectedErrorOccurred;  //"An unexpected error occurred"
                    return View("Error500");
            }
        }
        [IgnoreAction]
        //[NoLogging]
        //[AllowAnonymous] // AccessDenied
        public IActionResult AccessDeniedError403()
        {
            return View("AccessDeniedError403");
        }
        //[Route("Error")]
        //public IActionResult Error()
        //{
        //    var exceptionHandlerPathFeature =
        //        HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        //    // يمكنك تسجيل الخطأ في Logs هنا مثلاً:
        //    // _logger.LogError(exceptionHandlerPathFeature.Error, "Error at path: " + exceptionHandlerPathFeature.Path);

        //    return View("Error");
        //}
    }
}
