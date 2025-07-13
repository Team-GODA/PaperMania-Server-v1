using System.Security.Cryptography;
using Server.Infrastructure.Service.Interface;

namespace Server.Infrastructure.Service;

public class SessionService : ISessionService
{
    private ICacheService _cacheService;
    private readonly ILogger<SessionService> _logger;
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromHours(24);

    public SessionService(ICacheService cacheService, ILogger<SessionService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }
    
    public async Task<string> CreateSessionAsync(int userId)
    {
        var sessionId = GenerateSessionId();
        
        _logger.LogInformation($"세션 아이디 생성: 유저 아이디: {userId}, 세션 아이디: {sessionId}");
        
        await _cacheService.SetAsync(sessionId, userId.ToString(), _sessionTimeout);
        
        _logger.LogInformation($"[CreateSessionAsync] 세션 저장 완료: SessionId={sessionId}, TTL={_sessionTimeout}");
        
        return sessionId;
    }
    
    private string GenerateSessionId()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    public async Task<bool> ValidateSessionAsync(string sessionId)
    {
        var exists = await _cacheService.ExistsAsync(sessionId);
        
        _logger.LogInformation($"[ValidateSessionAsync] 세션 유효성 검사: SessionId={sessionId}, Exists={exists}");
        
        return exists;
    }

    public async Task<int?> GetUserIdBySessionAsync(string sessionId)
    {
        var value = await _cacheService.GetAsync(sessionId);
        
        if (value != null && int.TryParse(value, out var userId))
        {
            _logger.LogInformation($"[GetUserIdBySessionAsync] 세션으로부터 UserId 조회 성공: SessionId={sessionId}, UserId={userId}");
            return userId;
        }
        else
        {
            _logger.LogWarning($"[GetUserIdBySessionAsync] 세션으로부터 UserId 조회 실패: SessionId={sessionId}");
            return null;
        }
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        _logger.LogInformation($"[DeleteSessionAsync] 세션 삭제 요청: SessionId={sessionId}");
        
        await _cacheService.RemoveAsync(sessionId);
        
        _logger.LogInformation($"[DeleteSessionAsync] 세션 삭제 완료: SessionId={sessionId}");
    }
    
    public async Task RefreshSessionAsync(string sessionId)
    {
        bool exists = await _cacheService.ExistsAsync(sessionId);
        if (exists)
        {
            await _cacheService.SetExpirationAsync(sessionId, _sessionTimeout);
            _logger.LogInformation($"[RefreshSessionAsync] 세션 TTL 연장: SessionId={sessionId}, TTL={_sessionTimeout}");
        }
        else
        {
            _logger.LogWarning($"[RefreshSessionAsync] TTL 연장 실패: 세션 없음 또는 만료됨 SessionId={sessionId}");
        }
    }
}