using Application.Helpers;
using Application.Services.Admin;
using WebApplication.Helpers;
using WebApplication.Middleware;
using Infrastructure;
using Microsoft.Extensions.FileProviders;
using System.Globalization;
using WebApplication.Extensions;

namespace WebApplication.Extensions
{
    public static class PipelineExtensions
    {
        public static Microsoft.AspNetCore.Builder.WebApplication ConfigurePipeline(
            this Microsoft.AspNetCore.Builder.WebApplication app)
        {
            // Add LogsHistory middleware at the beginning to capture all requests
            app.UseLogsHistory();

            // Ensure required services are resolved correctly
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();
            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            FileRootProvider.Configure(env, app.Configuration);

            // Static helper initializations
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            var excelLogger = loggerFactory.CreateLogger("ExcelStaticReport");
            ExcelStaticReport.ConfigureExcel(env, excelLogger);
            SessionHelper.Configure(httpContextAccessor);
            SelectListHelper.Configure();
            RadioButtonHelper.Configure();

            // Error handling
            if (!app.Environment.IsDevelopment())
            {
                // General errors such as 500 and similar
                app.UseExceptionHandler("/Admin/Home/Error");

                // Error status codes such as 404 and 403
                app.UseStatusCodePagesWithReExecute(
                    "/CustomError/Handle",
                    "?statusCode={0}");

                // HSTS
                app.UseHsts();
            }
            else
            {
                // Detailed errors in Development
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Added from the second pipeline
            app.UseCookiePolicy();
            app.UseSecurityHeaders();
            app.UseRateLimiter();

            // Static files
            app.UseStaticFiles();

            // Uploaded files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(FileRootProvider.UploadsRootPath),
                RequestPath = "/uploads"
            });

            // Session
            app.UseSession();

            // Culture / Localization
            app.UseAppLocalization();
            
            app.Use(async (context, next) =>
            {
                var lang = context.Session.GetString("CurrentCulture");

                if (string.IsNullOrEmpty(lang))
                {
                    lang = "ar";
                    context.Session.SetString("CurrentCulture", lang);
                }

                var culture = new CultureInfo(lang);

                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                culture.NumberFormat.NumberDecimalSeparator = ".";
                culture.NumberFormat.CurrencyDecimalSeparator = ".";

                culture.DateTimeFormat.AMDesignator = "AM";
                culture.DateTimeFormat.PMDesignator = "PM";

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                await next();
            });

            app.UseRouting();

            // Authentication
            app.UseAuthentication();

            // Authorization
            app.UseAuthorization();

            // Existing WebApplication middleware
            app.UseMiddleware<LogsHistoryMiddleware>();
            app.UseMiddleware<NotificationMiddleware>();

            // Static assets
            app.MapStaticAssets();

            // Areas route - must come before default route
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            #region  Default route -- whithout Require Areas 
            //// Default route -- whithout Require Areas 
            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}")
            //    .WithStaticAssets();
            #endregion Root redirect to member Area

            // Default route - explicitly set area to Admin -- only with Admin Area
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}",
                defaults: new { area = "Admin" })
                .WithStaticAssets();

            // Root redirect to Admin area
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/Admin/Home/Index");
                return Task.CompletedTask;
            });

            #region Root redirect to member Area
            //// Root redirect
            //app.MapGet("/", context =>
            //{
            //    context.Response.Redirect("/member/home/index");
            //    return Task.CompletedTask;
            //});
            #endregion Root redirect to member Area 

            app.MapRazorPages();

            return app;
        }

        public static async Task SeedDatabaseAsync(
            this Microsoft.AspNetCore.Builder.WebApplication app)
        {
            await Seeder.SeedAdminUser(
                app.Services,
                app.Configuration);
        }
    }
}
