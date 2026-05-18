using Resend;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Abdulhadi.Foundation.Identity.Infrastructure.Services;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Infrastructure.Services.Email;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;
using Abdulhadi.Foundation.Identity.Infrastructure.Authentication.Jwt;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Authentication;

namespace Abdulhadi.Foundation.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        // 🟢 الحل الصحيح: إعداد مكتبة Resend وتمرير الـ ApiKey بشكل صريح
        services.AddResend(options =>
        {
            options.ApiToken = config["Resend:ApiKey"]!;
        });

        // ربط الإعدادات بكلاس الـ ResendOptions الخاص بك لقراءة الـ FromEmail
        services.Configure<ResendOptions>(config.GetSection("Resend"));

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IEmailService, ResendEmailService>();
        services.AddScoped<ISecurityCodeService, SecurityCodeService>();

        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));

        return services;
    }
}