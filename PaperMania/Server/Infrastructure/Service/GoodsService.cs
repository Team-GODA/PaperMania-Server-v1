using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class GoodsService : IGoodsService
{
    private readonly IGoodsRepository _goodsRepository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<GoodsService> _logger;

    public GoodsService(IGoodsRepository goodsRepository, ISessionService sessionService, ILogger<GoodsService> logger)
    {
        _goodsRepository = goodsRepository;
        _sessionService = sessionService;
        _logger = logger;
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
    
    public async Task<int> GetPlayerActionPointAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _goodsRepository.GetPlayerGoodsDataByUserIdAsync(userId);

        var currentActionPoint = data.ActionPoint;
        var maxActionPoint = data.MaxActionPoint;
        var lastRegenTime = data.LastActionPointUpdated;

        var nowUtc = DateTime.UtcNow;

        int regenIntervalSeconds = 120;
        int secondsPassed = (int)(nowUtc - lastRegenTime).TotalSeconds;
        int regenAmount = secondsPassed / regenIntervalSeconds;

        _logger.LogInformation($"현재 AP : {currentActionPoint} / MaxAP: {maxActionPoint}");

        if (regenAmount > 0 && currentActionPoint < maxActionPoint)
        {
            int apToAdd = Math.Min(regenAmount, maxActionPoint - currentActionPoint);
            currentActionPoint += apToAdd;
            data.LastActionPointUpdated = lastRegenTime.AddSeconds(apToAdd * regenIntervalSeconds);
            data.ActionPoint = currentActionPoint;

            _logger.LogInformation($"AP 증가: {apToAdd}, 새 AP: {currentActionPoint}");
            _logger.LogInformation($"LastActionPointUpdated 갱신: {data.LastActionPointUpdated}");

            await _goodsRepository.UpdatePlayerGoodsDataAsync(data);
        }

        return data.ActionPoint;
    }

    public async Task<int> UpdatePlayerMaxActionPoint(int userId, int newMaxActionPoint, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _goodsRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.MaxActionPoint = newMaxActionPoint;

        await _goodsRepository.UpdatePlayerGoodsDataAsync(data);
        return newMaxActionPoint;
    }

    public async Task<int> UpdatePlayerActionPointAsync(int userId, int newActionPoint, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var data = await _goodsRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        data.ActionPoint = newActionPoint;
        
        await _goodsRepository.UpdatePlayerGoodsDataAsync(data);
        return newActionPoint;
    }
}