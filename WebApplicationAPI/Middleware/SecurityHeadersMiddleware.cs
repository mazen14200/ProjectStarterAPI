using WebApplicationAPI.Models;

namespace WebApplicationAPI.Middleware;

/// <summary>
/// Adds OWASP-recommended security headers to every response and strips
/// headers that disclose server/framework information.
/// For a Web API, CSP is mostly relevant if you ever serve Swagger UI,
/// error pages, or any HTML — but it's cheap insurance either way.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // --- Remove information-disclosure headers ---
            headers.Remove("Server");
            headers.Remove("X-Powered-By");
            headers.Remove("X-AspNet-Version");
            headers.Remove("X-AspNetMvc-Version");

            // --- Clickjacking / MIME sniffing ---
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";

            // Legacy header, harmless to keep for older browsers
            headers["X-XSS-Protection"] = "1; mode=block";

            // --- Referrer / feature policy ---
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] =
                "geolocation=(), camera=(), microphone=(), payment=(), usb=(), " +
                "magnetometer=(), gyroscope=(), fullscreen=(self)";



            // ---------------------------------------------------------------------
            // === Content Security Policy (CSP)  --- Content-Security-Policy --- ===
            // For a pure JSON API this mainly protects Swagger UI / any HTML
            // error pages. Tighten/loosen per your actual served content.
            // ---------------------------------------------------------------------
            // Keep a strict CSP for the application by default.
            //
            // Scalar is an interactive JavaScript-based API documentation UI.
            // Depending on the Scalar UI version, it may require inline scripts/styles,
            // so we apply a separate, limited CSP only to the /scalar path.
            //
            // IMPORTANT:
            // Do NOT add 'unsafe-inline' to the global/default CSP because that would
            // weaken the security of the entire MVC/API application.
            // ---------------------------------------------------------------------

            //            Application
            //                 │
            //   ┌──────────────┼─────────────-─┐
            //    │              │               │
            //    MVC API           Scalar
            //      │              │               │
            //     / Home / api / v1 / scalar / v1
            //      / Products / api / v1 / ...      │
            //         │              │               │
            //      Strict CSP    Strict CSP      Scalar CSP

            if (!headers.ContainsKey("Content-Security-Policy"))
            {
                if (context.Request.Path.StartsWithSegments("/scalar")) // Scalar only Allowed Has Add Any CDN or Script/Style Inline, so we need to allow that in the CSP for Scalar
                {
                    headers["Content-Security-Policy"] =
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline'; " +
                        "style-src 'self' 'unsafe-inline'; " +
                        "img-src 'self' data: blob:; " +
                        "font-src 'self' data:; " +
                        "connect-src 'self'; " +
                        "object-src 'none'; " +
                        "frame-ancestors 'none'; " +
                        "base-uri 'self'";
                }
                else // Reamain System Strict CSP for the rest of the application
                {
                    headers["Content-Security-Policy"] =
                        "default-src 'self'; " +
                        "script-src 'self'; " +
                        "style-src 'self'; " +
                        "img-src 'self' data:; " +
                        "font-src 'self'; " +
                        "connect-src 'self'; " +
                        "object-src 'none'; " +
                        "frame-ancestors 'none'; " +
                        "base-uri 'self'";
                }
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}

public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
