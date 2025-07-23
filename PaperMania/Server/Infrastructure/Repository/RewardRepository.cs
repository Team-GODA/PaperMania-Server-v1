using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class RewardRepository : RepositoryBase, IRewardRepository
{
    public RewardRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<StageReward?> GetStageRewardByUserIdAsync(int userId, int stageNum, int stageSubNum)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT S.stage_num AS StageNum, 
               S.stage_sub_num AS StageSubNum, 
               S.clear_paper_piece AS PaperPiece, 
               S.clear_gold AS Gold, 
               S.clear_exp AS ClearExp
        FROM paper_mania_stage_data.stage_reward S
        JOIN paper_mania_stage_data.player_stage_data PS 
            ON S.stage_num = PS.stage_num 
            AND S.stage_sub_num = PS.stage_sub_num
        WHERE PS.id = @UserId 
          AND S.stage_num = @StageNum 
          AND S.stage_sub_num = @SubStageNum";

        var result = await db.QueryFirstOrDefaultAsync<StageReward>(sql, new
        {
            UserId  = userId,
            StageNum = stageNum,
            SubStageNum = stageSubNum
        });
        
        return result;
    }
}