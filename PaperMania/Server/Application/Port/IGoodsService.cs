using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IGoodsService
{
    Task<int> GetPlayerActionPointAsync(int userId, string sessionId);
    Task<int> UpdatePlayerMaxActionPoint(int userId, int newMaxActionPoint, string sessionId);
    Task<int> UpdatePlayerActionPointAsync(int userId, int newActionPoint, string sessionId);
}