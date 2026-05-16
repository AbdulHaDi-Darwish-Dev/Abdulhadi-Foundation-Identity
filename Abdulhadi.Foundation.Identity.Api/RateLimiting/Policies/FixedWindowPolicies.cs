using Microsoft.AspNetCore.RateLimiting;
using Abdulhadi.Foundation.Identity.Api.RateLimiting.Constants;

namespace Abdulhadi.Foundation.Identity.Api.RateLimiting.Policies;

public static class FixedWindowPolicies
{
    public static void Configure(RateLimiterOptions options)
    {
        options.AddFixedWindowLimiter(RateLimitPolicies.Default_150, config =>
        {
            config.PermitLimit = 150;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter(RateLimitPolicies.General_40, config =>
        {
            config.PermitLimit = 40;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter(RateLimitPolicies.Register_3, config =>
        {
            config.PermitLimit = 3;
            config.Window = TimeSpan.FromHours(1);
            config.QueueLimit = 0;
        });

        options.AddFixedWindowLimiter(RateLimitPolicies.PublicApi_60, config =>
        {
            config.PermitLimit = 60;
            config.Window = TimeSpan.FromMinutes(1);
            config.QueueLimit = 0;
        });
    }
}