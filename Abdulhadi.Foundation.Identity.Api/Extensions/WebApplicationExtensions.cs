using BuildingBlocks.Logging.Extensions;
using Abdulhadi.Foundation.Identity.Api.Middleware;

namespace Abdulhadi.Foundation.Identity.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UsePresentation(this WebApplication app)
    {
        // توليد الـ Correlation ID للطلب في البداية
        app.UseCorrelationId();

        app.UseHttpsRedirection();
        app.UseCorsPolicy();
        app.UseRateLimiting();

        // التعرف على المستخدم
        app.UseSecurity();

        // إثراء الـ Logs بالـ User والـ Correlation وكتابة الـ Log
        app.UseLoggingEnrichment();
        app.UseMiddleware<RequestLoggingMiddleware>();

        // الـ Endpoints
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