using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardRepository
{
    Task<StageReward?> GetStageRewardByUserIdAsync(int userId, StageReward reward);
}