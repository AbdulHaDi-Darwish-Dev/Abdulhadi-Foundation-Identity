using BuildingBlocks.Auditing.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Auditing.Extensions;

public static class AuditingExtensions
{
    public static IServiceCollection AddSharedAuditing(this IServiceCollection services)
    {
        services.AddScoped<AuditableInterceptor>();

        return services;
    }
}