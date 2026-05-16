using OpenTelemetry.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Monitoring;

public static class MonitoringServiceCollectionExtensions
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. تفعيل نظام الـ Health Checks الأساسي
        services.AddHealthChecks();

        // 2. تفعيل OpenTelemetry للمقاييس (Metrics)
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation() // تتبع أداء الـ API
                    .AddHttpClientInstrumentation() // تتبع الـ HTTP Requests الخارجية
                    .AddRuntimeInstrumentation()   // تتبع أداء بيئة التشغيل (.NET Runtime)
                    .AddPrometheusExporter();     // تصدير البيانات لتطبيق Prometheus
            });

        return services;
    }
}