using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IRewardService
{
    Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum);
}