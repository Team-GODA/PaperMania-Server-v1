namespace Server.Infrastructure.Service.Interface;

public interface ICacheService
{
    Task SetAsync(string key, string value, TimeSpan? expiration = null);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}