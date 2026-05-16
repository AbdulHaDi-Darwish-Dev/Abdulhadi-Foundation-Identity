using BuildingBlocks.Identity.Options;
using BuildingBlocks.Identity.Services;
using BuildingBlocks.Identity.Providers;
using Microsoft.Extensions.Configuration;
using BuildingBlocks.Identity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddExternalAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. ربط الإعدادات (Options Pattern)
        services.Configure<GoogleOptions>(configuration.GetSection(GoogleOptions.SectionName));

        // 2. تسجيل مزودي الخدمة (Providers)
        services.AddScoped<IExternalAuthProvider, GoogleAuthProvider>();

        // 3. تسجيل المحرك المركزي (Manager)
        services.AddScoped<ExternalAuthManager>();

        return services;
    }
}