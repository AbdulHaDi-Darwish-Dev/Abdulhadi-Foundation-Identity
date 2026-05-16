using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Abdulhadi.Foundation.Identity.Infrastructure.Services;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddMemoryCache();

        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }
}