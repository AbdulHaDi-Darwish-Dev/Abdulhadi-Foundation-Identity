using BuildingBlocks.Shared.Contracts;
using Abdulhadi.Foundation.Identity.Api.RateLimiting.Policies;

namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            FixedWindowPolicies.Configure(options);

            SlidingWindowPolicies.Configure(options);

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var response = ApiResponse<object>.Fail(
                    "Too many requests.",
                    "RATE_LIMIT_EXCEEDED");

                await context.HttpContext.Response.WriteAsJsonAsync(response, token);
            };
        });

        return services;
    }

    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseRateLimiter();
    }
}