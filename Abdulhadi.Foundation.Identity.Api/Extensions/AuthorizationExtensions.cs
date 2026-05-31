using Abdulhadi.Foundation.Identity.Api.Constants;

namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityPermissions.RevokeSessions, policy =>
                policy.RequireRole("Owner", "Admin"));
        });

        return services;
    }
}