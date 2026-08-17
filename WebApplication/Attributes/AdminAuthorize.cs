using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminAuthorizeAttribute : TypeFilterAttribute
    {
        public AdminAuthorizeAttribute() : base(typeof(AdminAuthorizeFilter))
        {
        }
    }

    public class AdminAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AdminAuthorizeFilter(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            var isAdminSignedIn = user?.Identity?.IsAuthenticated == true && _signInManager.IsSignedIn(user);
            if (isAdminSignedIn)
            {
                return Task.CompletedTask;
            }

            context.Result = new RedirectToActionResult("Login", "Account", new { area = "Identity" });
            return Task.CompletedTask;
        }
    }
}
