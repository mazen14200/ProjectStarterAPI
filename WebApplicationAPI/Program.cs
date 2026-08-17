using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using WebApplicationAPI.Controllers;
using WebApplicationAPI.Extensions;
using WebApplicationAPI.Filters;
using WebApplicationAPI.Middleware;
using WebApplicationAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Kestrel: don't advertise the server in the "Server" header
// ---------------------------------------------------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

// ---------------------------------------------------------------------
// Services
// // Supports BOTH:
// // 1. API Controllers -> [ApiController]
// // 2. MVC Controllers -> Controller + Views
// ---------------------------------------------------------------------

builder.Services
    .AddControllersWithViews(options =>
    {
        // Global validation filter
        options.Filters.Add<ValidateModelAttribute>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // We handle validation ourselves
        options.SuppressModelStateInvalidFilter = true;
    });

// Needed for API Explorer / OpenAPI
builder.Services.AddEndpointsApiExplorer();


// ---------------------------------------------------------------------
// OpenAPI + Scalar
// ---------------------------------------------------------------------
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});


// ---------------------------------------------------------------------
// JWT Authentication / Authorization
// CORS
// Rate Limiting
// HSTS
// ---------------------------------------------------------------------
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProductionCors(builder.Configuration);
builder.Services.AddApiRateLimiting();
builder.Services.AddStrictHsts();
// ---------------------------------------------------------------------
// Application Services
// // --- Auth building blocks (swap in-memory implementations for real
// //    repositories backed by your database when ready) ---
// ---------------------------------------------------------------------
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();
builder.Services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
builder.Services.AddScoped<ITokenService, TokenService>();

// ---------------------------------------------------------------------
// Antiforgery is included for completeness if any cookie-authenticated,
// browser-driven endpoints are ever added. Pure bearer-token JSON endpoints
// don't need it since there's no ambient credential a browser attaches
// automatically.
// ---------------------------------------------------------------------

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "__Host-XSRF-TOKEN";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = false; // must be readable by JS to echo back in a header
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.HeaderName = "X-XSRF-TOKEN";
});

// ---------------------------------------------------------------------
// Cookie Policy
// // If the API ever issues its own auth cookies (e.g. refresh token), lock them down.
// ---------------------------------------------------------------------
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

// ---------------------------------------------------------------------
// ProblemDetails
// // Consistent problem-details error shape for unhandled exceptions
// ---------------------------------------------------------------------
builder.Services.AddProblemDetails();

var app = builder.Build();

// =====================================================================
// HTTP PIPELINE Pipeline (order matters)
// =====================================================================

// ### 1. Global Exception Handler -> ProblemDetails, no stack traces to the client
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Instance = context.Request.Path
        };
        await context.Response.WriteAsJsonAsync(problem);
    });
});

// ### 2. HTTPS + HSTS
app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// ### 3.  Static Files - wwwroot
app.UseStaticFiles();


//// ### 4. Security headers + info-disclosure header removal  <<<<<-------  For JS Removal
app.UseCustomSecurityHeaders();

// ---------------------------------------------------------------------
// ###  5. OpenAPI document + Scalar UI
// OpenAPI + Scalar
// Development only
// ---------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();    // serves /openapi/v1.json

    app.MapScalarApiReference(options =>
    {
        options.Title = "Secure Web API";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient =
            new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
    //app.MapScalarApiReference(options =>
    //{
    //    options.AddDocument(
    //        "v1",
    //        "WebApplicationAPI",
    //        "/openapi/v1.json");
    //});
}


// ---------------------------------------------------------------------
// Routing
// ---------------------------------------------------------------------
app.UseRouting();

// ---------------------------------------------------------------------
// CORS
// ---------------------------------------------------------------------
app.UseCors("ProductionCors");

// ---------------------------------------------------------------------
// Rate Limiting
// ---------------------------------------------------------------------
app.UseRateLimiter();

// ---------------------------------------------------------------------
// Cookie Policy
// ---------------------------------------------------------------------
app.UseCookiePolicy();

// ---------------------------------------------------------------------
// Authentication / Authorization
// ---------------------------------------------------------------------
app.UseAuthentication();
app.UseAuthorization();

// =====================================================================
// ###  6. No-cache for API responses
// //    No-store cache headers on all API responses by default (JSON API responses
// //    should generally not be cached by intermediaries/browsers)
// =====================================================================
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (!context.Response.Headers.ContainsKey("Cache-Control"))
        {
            context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
            context.Response.Headers.Pragma = "no-cache";
        }
        return Task.CompletedTask;
    });
    await next();
});

// =====================================================================
// MVC ROUTING
// =====================================================================
//
// Example:
// /Products
// /Products/Index
// /Products/Details/5
//
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =====================================================================
// API CONTROLLERS
// =====================================================================
//
// Controllers using:
// [ApiController]
// [Route("api/v1/[controller]")]
//
// Example:
// POST /api/v1/Auth/login
// GET  /api/v1/Auth/me

app.MapControllers();

app.Run();
