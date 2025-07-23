using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardRepository
{
    Task<StageReward?> GetStageRewardAsync(StageData stage);
}