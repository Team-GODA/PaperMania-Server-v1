using Server.Domain.Entity;

namespace Server.Application.Port;

public interface ICurrencyService
{
    Task<int> GetPlayerActionPointAsync(int? userId, string sessionId);
    Task<int> UpdatePlayerMaxActionPoint(int? userId, int newMaxActionPoint, string sessionId);
    Task UsePlayerActionPointAsync(int? userId, int usedActionPoint, string sessionId);
    
    Task<int> GetPlayerGoldAsync(int? userId, string sessionId);
    Task AddPlayerGoldAsync(int userId, int gold, string sessionId);
    Task UsePlayerGoldAsync(int? userId, int usedGold, string sessionId);
}