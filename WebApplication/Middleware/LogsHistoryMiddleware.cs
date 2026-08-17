using Domain.Entities;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Middleware
{
    public class LogsHistoryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogsHistoryMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;

        public LogsHistoryMiddleware(RequestDelegate next, ILogger<LogsHistoryMiddleware> logger, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            string errorMessage = null;

            // Skip logging for static files
            if (IsStaticFileRequest(context))
            {
                await _next(context);
                return;
            }

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.LogError(ex, "An error occurred during request processing");
                throw;
            }
            finally
            {
                stopwatch.Stop();

                // Create scope to get DbContext
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var log = new LogsHistory
                    {
                        OperationType = context.Request.Method,
                        OperationName = GetOperationName(context),
                        Path = context.Request.Path,
                        Method = context.Request.Method,
                        StatusCode = context.Response.StatusCode,
                        UserName = context.User?.Identity?.Name,
                        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                        UserAgent = context.Request.Headers["User-Agent"].ToString(),
                        CreatedAt = DateTime.UtcNow,
                        ErrorMessage = errorMessage,
                        DurationMs = stopwatch.ElapsedMilliseconds
                    };

                    try
                    {
                        dbContext.LogsHistories.Add(log);
                        await dbContext.SaveChangesAsync();

                        // Log to console
                        _logger.LogInformation("Request logged: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
                            log.Method, log.Path, log.StatusCode, log.DurationMs);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save log to database");
                    }
                }
            }
        }

        private bool IsStaticFileRequest(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Skip static files with common extensions
            var staticExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2", ".ttf", ".eot", ".map" };
            if (staticExtensions.Any(ext => path?.EndsWith(ext) == true))
            {
                return true;
            }

            // Skip common static file directories
            var staticDirectories = new[] { "/lib/", "/css/", "/js/", "/images/", "/fonts/", "/wwwroot/" };
            if (staticDirectories.Any(dir => path?.StartsWith(dir) == true))
            {
                return true;
            }

            // Skip favicon
            if (path == "/favicon.ico")
            {
                return true;
            }

            return false;
        }

        private string GetOperationName(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerAction = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                if (controllerAction != null)
                {
                    return $"{controllerAction.ControllerName}/{controllerAction.ActionName}";
                }
            }
            return context.Request.Path;
        }
    }

    public static class LogsHistoryMiddlewareExtensions
    {
        public static IApplicationBuilder UseLogsHistory(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogsHistoryMiddleware>();
        }
    }
}
