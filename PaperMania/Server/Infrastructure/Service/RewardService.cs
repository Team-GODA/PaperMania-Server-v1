﻿using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class RewardService : IRewardService
{
    private readonly IRewardRepository _rewardRepository;
    private readonly IStageRepository _stageRepository;

    public RewardService(IRewardRepository rewardRepository, IStageRepository stageRepository)
    {
        _rewardRepository = rewardRepository;
        _stageRepository = stageRepository;
    }
    
    public async Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum)
    {
        return await _rewardRepository.GetStageRewardAsync(stageNum, stageSubNum);
    }

    public async Task ClaimStageRewardByUserIdAsync(int userId, StageReward reward, PlayerStageData data)
    {
        var stageReward = await _rewardRepository.GetStageRewardAsync(data.StageNum, data.SubStageNum);
        if (stageReward == null)
            throw new InvalidOperationException("해당 스테이지 보상을 찾을 수 없습니다.");
        
        if (await _stageRepository.IsClearedStageAsync(data))
            stageReward.PaperPiece = 0;
        else
        {
            data.IsCleared = true;
            await _stageRepository.UpdateIsClearedAsync(data);
        }
        
        await _rewardRepository.ClaimStageRewardByUserIdAsync(userId, stageReward);
    }
}