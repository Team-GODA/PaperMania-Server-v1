using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardService
{
    Task<StageReward?> GetStageRewardByUserIdAsync(int userId, int stageNum, int stageSubNum);
}