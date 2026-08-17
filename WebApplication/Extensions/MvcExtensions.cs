using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.Extensions
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            
            // Add localization services
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            return services;
        }
    }
}