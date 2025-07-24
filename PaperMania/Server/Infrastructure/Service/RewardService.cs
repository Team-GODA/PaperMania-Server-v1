using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class RewardService : IRewardService
{
    private readonly IRewardRepository _rewardRepository;

    public RewardService(IRewardRepository rewardRepository)
    {
        _rewardRepository = rewardRepository;
    }
    
    public async Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum)
    {
        return await _rewardRepository.GetStageRewardAsync(stageNum, stageSubNum);
    }
    
    
}