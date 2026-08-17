using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApplication.Helpers
{
    public class BypassAntiforgery : IAntiforgery
    {
        public AntiforgeryTokenSet GetAndStoreTokens(HttpContext httpContext)
        {
            return new AntiforgeryTokenSet("dummy-token", "dummy-cookie", "__RequestVerificationToken", "dummy-header");
        }

        public AntiforgeryTokenSet GetTokens(HttpContext httpContext)
        {
            return new AntiforgeryTokenSet("dummy-token", "dummy-cookie", "__RequestVerificationToken", "dummy-header");
        }

        public Task<bool> IsRequestValidAsync(HttpContext httpContext)
        {
            return Task.FromResult(true);
        }

        public Task ValidateRequestAsync(HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        public void SetCookieTokenAndHeader(HttpContext httpContext)
        {
            // No-op
        }
    }
}
