using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MemberOrAdminAuthorizeAttribute : TypeFilterAttribute
    {
        public MemberOrAdminAuthorizeAttribute() : base(typeof(MemberOrAdminAuthorizeFilter))
        {
        }
    }

    public class MemberOrAdminAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MemberOrAdminAuthorizeFilter(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;
            var session = httpContext.Session;

            var isAdminSignedIn = user?.Identity?.IsAuthenticated == true && _signInManager.IsSignedIn(user);
            var isMemberSignedIn =
                !string.IsNullOrWhiteSpace(session.GetString("Email")) &&
                !string.IsNullOrWhiteSpace(session.GetString("AuthToken"));

            if (isAdminSignedIn || isMemberSignedIn)
            {
                return Task.CompletedTask;
            }

            context.Result = new RedirectToActionResult("Login", "Account", new { area = "Identity" });
            return Task.CompletedTask;
        }
    }
}