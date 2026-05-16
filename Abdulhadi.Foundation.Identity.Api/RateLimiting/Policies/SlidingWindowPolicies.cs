using Microsoft.AspNetCore.RateLimiting;
using Abdulhadi.Foundation.Identity.Api.RateLimiting.Constants;

namespace Abdulhadi.Foundation.Identity.Api.RateLimiting.Policies;

public static class SlidingWindowPolicies
{
    public static void Configure(RateLimiterOptions options)
    {
        options.AddSlidingWindowLimiter(
            RateLimitPolicies.Login_5,
            config =>
            {
                config.PermitLimit = 5;

                config.Window = TimeSpan.FromMinutes(1);

                config.SegmentsPerWindow = 6;

                config.QueueLimit = 0;
            });

        options.AddSlidingWindowLimiter(
            RateLimitPolicies.RefreshToken_10,
            config =>
            {
                config.PermitLimit = 10;

                config.Window = TimeSpan.FromMinutes(1);

                config.SegmentsPerWindow = 6;

                config.QueueLimit = 0;
            });

        options.AddSlidingWindowLimiter(
            RateLimitPolicies.ForgotPassword_3,
            config =>
            {
                config.PermitLimit = 3;

                config.Window = TimeSpan.FromHours(1);

                config.SegmentsPerWindow = 6;

                config.QueueLimit = 0;
            });
    }
}