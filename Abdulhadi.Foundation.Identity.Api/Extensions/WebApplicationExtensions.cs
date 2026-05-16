using BuildingBlocks.Logging.Extensions;
using Abdulhadi.Foundation.Identity.Api.Middleware;

namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UsePresentation(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseCorsPolicy();

        app.UseSecurity();

        app.UseSharedLogging(); // 🔥 أول شيء (Correlation + Context)
        app.UseMiddleware<RequestLoggingMiddleware>(); // 🔥 بعده مباشرة

        app.UseRateLimiting();

        app.MapHealthChecks("/health");
        app.MapPrometheusScrapingEndpoint("/metrics");

        app.MapControllers();

        return app;
    }

    public static WebApplication UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}