using System.Security.Cryptography;
using Server.Infrastructure.Service.Interface;

namespace Server.Infrastructure.Service;

public class SessionService : ISessionService
{
    private ICacheService _cacheService;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(24);

    public SessionService(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        var sessionId = GenerateSessionId();
        await _cacheService.SetAsync(sessionId, userId.ToString(), _sessionTimeout);
        return sessionId;
    }
    
    private string GenerateSessionId()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    public async Task<bool> ValidateSessionAsync(string sessionId)
    {
        return await _cacheService.ExistsAsync(sessionId);
    }

    public async Task<int?> GetUserIdBySessionAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(sessionId);
        return value != null && int.TryParse(value, out var userId) ? userId : null;
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        await _cacheService.RemoveAsync(sessionId);
    }
}