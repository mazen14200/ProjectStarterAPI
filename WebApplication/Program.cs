// SECURITY FIX: Add using statements to resolve namespace conflict
// The project name "WebApplication" conflicts with ASP.NET Core's WebApplication class
using Microsoft.AspNetCore.Builder;
using webApplication = Microsoft.AspNetCore.Builder.WebApplication;
using WebApplication.Extensions;

var builder = webApplication.CreateBuilder(args);

// Add services to the container using extension methods
builder.Services.AddMvcServices();
builder.Services.AddSecurityServices();
builder.Services.AddIdentityServices();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Seed admin user
await app.SeedDatabaseAsync();

// Configure the HTTP request pipeline using extension method
app.ConfigurePipeline();

app.Run();
