using Serilog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using BuildingBlocks.Logging.Context;
using BuildingBlocks.Logging.Middleware;
using BuildingBlocks.Logging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Logging.Serilog.Configuration;

namespace BuildingBlocks.Logging.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddSharedLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
        builder.Services.AddScoped<IRequestContext, RequestContext>();
        builder.Services.AddHttpContextAccessor();

        builder.Host.UseSerilog((ctx, services, lc) =>
        {
            // استدعاء التهيئة المركزية وتمرير الكائن lc
            SerilogConfiguration.Configure(lc, ctx.Configuration);

            // إضافة الخصائص المخصصة للتطبيق بدون تكرار
            lc.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName);
        });

        return builder;
    }

    // نستخدم هذا في أول الـ Pipeline
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationMiddleware>();
    }

    // نستخدم هذا بعد الـ Authentication
    public static IApplicationBuilder UseLoggingEnrichment(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestEnrichmentMiddleware>();
    }
}