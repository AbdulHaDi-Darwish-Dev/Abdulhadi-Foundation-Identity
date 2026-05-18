using Microsoft.Extensions.DependencyInjection;
using Abdulhadi.Foundation.Identity.Application.Services;
using Abdulhadi.Foundation.Identity.Application.Security.OTP;
using Abdulhadi.Foundation.Identity.Application.Security.Tokens;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IOtpService, OtpService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}