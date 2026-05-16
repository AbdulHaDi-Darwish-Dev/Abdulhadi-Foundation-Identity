using Microsoft.Extensions.Caching.Memory;
using Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

namespace Abdulhadi.Foundation.Identity.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiry.HasValue)
        {
            options.SetAbsoluteExpiration(expiry.Value);
        }

        _cache.Set(key, value, options);

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}