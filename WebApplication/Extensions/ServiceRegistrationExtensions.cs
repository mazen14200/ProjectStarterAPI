using almetsaweq.Application.ServiceInterfaces;
using almetsaweq.Application.Services;
using Application.Interfaces;
using Application.ServiceInterfaces;
using Application.Services;
using Infrastructure.InterfacesDB;
using Infrastructure.InterfacesDB.RemainInterfacesDB;
using Infrastructure.Repositories;
using Infrastructure.Repositories.RemainRepositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApplication.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper
            services.AddAutoMapper(cfg => {
                cfg.AddProfile<Application.Mappings.MappingProfile>();
            });

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IRoleRepository, RoleRepository>();

            // Register your custom services, repositories, and other dependencies here
            // Example:
            services.AddScoped<IExampleService, ExampleService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleClaimsService, RoleClaimsService>();
            // services.AddTransient<IMyTransientService, MyTransientService>();
            // services.AddSingleton<IMySingletonService, MySingletonService>();

            return services;
        }
    }
}
