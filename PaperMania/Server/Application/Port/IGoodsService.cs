using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IGoodsService
{
    Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int userId, string sessionId);
    Task UpdatePlayerGoodsDataAsync(PlayerGoodsData data, string sessionId);
    Task<int> GetPlayerActionPointAsync(int userId, string sessionId);
}