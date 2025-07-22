using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class GoodsService : IGoodsService
{
    private readonly IGoodsRepository _goodsRepository;
    private readonly ISessionService _sessionService;

    public GoodsService(IGoodsRepository goodsRepository, ISessionService sessionService)
    {
        _goodsRepository = goodsRepository;
        _sessionService = sessionService;
    }
    
    private async Task ValidateSessionAsync(string sessionId)
    {
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        if (userId == null)
            throw new Exception($"세션 ID에 맞는 유저 ID가 없습니다. : Id : {userId}");
        
        var isValid = await _sessionService.ValidateSessionAsync(sessionId, userId.Value);
        if (!isValid)
            throw new UnauthorizedAccessException("세션이 유효하지 않습니다.");
    }
    
    public async Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await _goodsRepository.GetPlayerGoodsDataByUserIdAsync(userId);

        return data;
    }

    public async Task UpdatePlayerGoodsDataAsync(PlayerGoodsData data, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        await _goodsRepository.UpdatePlayerGoodsDataAsync(data);
    }
}