namespace Abdulhadi.Foundation.Identity.Application.Abstractions.Services;

public interface ICacheService
{
    Task RemoveAsync(string key);
    Task<T?> GetAsync<T>(string key);
    Task<bool> ExistsAsync(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
}