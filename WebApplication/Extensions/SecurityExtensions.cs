using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace WebApplication.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services)
        {
            // SECURITY FIX: Configure cookie policy to enforce Secure, HttpOnly, and SameSite settings
            services.AddCookiePolicy(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // SECURITY FIX: Configure HSTS with strict settings for production
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            // SECURITY FIX: Configure CORS to restrict allowed origins
            services.AddCors(options =>
            {
                options.AddPolicy("ProductionCors", policy =>
                {
                    policy.WithOrigins("https://yourdomain.com")
                          .WithHeaders("Content-Type", "Authorization")
                          .WithMethods("GET", "POST", "PUT", "DELETE")
                          .AllowCredentials();
                });
            });

            // SECURITY FIX: Configure session security
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;
            });

            // SECURITY FIX: Add Anti-Forgery (CSRF) protection explicitly
            services.AddAntiforgery(options =>
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.HeaderName = "X-CSRF-TOKEN";
            });

            // SECURITY FIX: Add Rate Limiting
            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 requests
                            QueueLimit = 2,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            Window = TimeSpan.FromMinutes(1) // per 1 minute
                        }));
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                // SECURITY FIX: Remove Server header to hide server information
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Remove("Server");
                    context.Response.Headers.Remove("X-Powered-By");
                    context.Response.Headers.Remove("X-AspNet-Version");
                    context.Response.Headers.Remove("X-AspNetMvc-Version");
                    return Task.CompletedTask;
                });

                // SECURITY FIX: Add Content Security Policy (CSP) header (Improved for ASP.NET Core UI)
                context.Response.Headers.Append("Content-Security-Policy",
                    "default-src 'self'; " +
                    "script-src 'self' 'unsafe-inline' https://cdnjs.cloudflare.com https://cdn.jsdelivr.net; " +
                    "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                    "img-src 'self' data: https:; " +
                    "font-src 'self' data: https://cdnjs.cloudflare.com; " +
                    "object-src 'none'; " +
                    "frame-ancestors 'none'; " +
                    "base-uri 'self';");

                // SECURITY FIX: Add X-Content-Type-Options: nosniff
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

                // SECURITY FIX: Add Referrer-Policy
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

                // SECURITY FIX: Add Permissions-Policy
                context.Response.Headers.Append("Permissions-Policy",
                    "geolocation=(), microphone=(), camera=(), payment=(), usb=(), magnetometer=(), gyroscope=()");

                // SECURITY FIX: Add X-Frame-Options: DENY
                context.Response.Headers.Append("X-Frame-Options", "DENY");

                // SECURITY FIX: Add X-XSS-Protection
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

                await next();
            });
        }
    }
}