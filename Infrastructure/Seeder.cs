using Infrastructure.Identity;
using Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Seeder
    {
        public static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database is created
            dbContext.Database.EnsureCreated();

            // Get admin credentials from appsettings.json
            var adminPassword = configuration["Admin:password"] ?? "Super@123";
            var adminEmail = "admin@example.com";
            var adminUserName = "admin";

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole("Admin"));
            }

            // Create admin user if it doesn't exist
            var adminUser = await userManager.FindByNameAsync(adminUserName);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            // Create Master user if it doesn't exist
            var masterPassword ="Master@123";
            var masterEmail = "Master@test.com";
            var masterUserName = "master";
            var masterUser = await userManager.FindByNameAsync(masterUserName);
            if (masterUser == null)
            {
                masterUser = new ApplicationUser
                {
                    UserName = masterUserName,
                    Email = masterEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(masterUser, masterPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(masterUser, "Admin");
                }
                else
                {
                    throw new Exception($"Failed to create master user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
