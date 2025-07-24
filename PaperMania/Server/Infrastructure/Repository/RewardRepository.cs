using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class RewardRepository : RepositoryBase, IRewardRepository
{
    public RewardRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<StageReward?> GetStageRewardAsync(int stageNum, int stageSubNum)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT stage_num AS StageNum, 
               stage_sub_num AS SubStageNum, 
               clear_paper_piece AS PaperPiece, 
               clear_gold AS Gold, 
               clear_exp AS ClearExp
        FROM paper_mania_stage_data.stage_reward 
        WHERE stage_num = @StageNum 
          AND stage_sub_num = @SubStageNum";

        var result = await db.QueryFirstOrDefaultAsync<StageReward>(sql, new
        {
            StageNum = stageNum,
            SubStageNum = stageSubNum
        });
        
        return result;
    }
}