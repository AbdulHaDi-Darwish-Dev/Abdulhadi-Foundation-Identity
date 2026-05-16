using Microsoft.Extensions.DependencyInjection;
using Abdulhadi.Foundation.Identity.Application.Security.Tokens;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;

namespace Abdulhadi.Foundation.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOtpService, OtpService>();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}