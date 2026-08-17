using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace WebApplicationAPI.Extensions;

public static class SecurityServiceExtensions
{
    /// <summary>
    /// JWT Bearer authentication — the standard fit for a stateless Web API
    /// (as opposed to the cookie-based Identity UI in the MVC guide).
    /// Because there's no browser-managed session cookie, CSRF tokens are
    /// not needed for bearer-token endpoints; just keep tokens out of storage
    /// that's readable by XSS (avoid localStorage for the token when a SPA
    /// client is involved — prefer an httpOnly cookie or in-memory storage).
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSection = config.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true; // never allow HTTP metadata in production
            options.SaveToken = false;            // don't keep raw token in AuthenticationProperties
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSection["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSection["Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1) // keep tight; default 5 min is often too generous
            };

            // Prevent leaking exception details via the WWW-Authenticate header
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.NoResult();
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.Response.ContentType = "application/problem+json";
                    return ctx.Response.WriteAsync(
                        "{\"title\":\"Unauthorized\",\"status\":401}");
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Restricted CORS policy — no AllowAnyOrigin in production. Configure
    /// allowed origins in appsettings (Cors:AllowedOrigins).
    /// </summary>
    public static IServiceCollection AddProductionCors(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy("ProductionCors", policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .SetIsOriginAllowedToAllowWildcardSubdomains()
                          .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                          .WithHeaders("Content-Type", "Authorization")
                          .AllowCredentials();
                }
                else
                {
                    // No origins configured: fail closed, not open.
                    policy.WithOrigins(Array.Empty<string>());
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Basic fixed-window rate limiting to blunt brute-force / flooding
    /// attacks (built into ASP.NET Core, no extra package needed).
    /// Apply the "auth" policy specifically to login/register endpoints.
    /// </summary>
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // Tighter policy for auth endpoints — brute-force mitigation
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

        return services;
    }

    /// <summary>
    /// HSTS with strict, preload-ready settings (Priority 1 fix from the
    /// security guide, unchanged for an API).
    /// </summary>
    public static IServiceCollection AddStrictHsts(this IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        return services;
    }
}
