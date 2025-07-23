using Dapper;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class StageRepository : RepositoryBase, IStageRepository
{
    public StageRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task CreatePlayerStageDataAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
                INSERT INTO paper_mania_stage_data.player_stage_data (id, stage_num, stage_sub_num, is_cleared)
                VALUES (@Id, @StageNum, @SubStageNum, false);
            ";
        
        var data = new List<dynamic>();
        for (int stageNum = 1; stageNum <= 5; stageNum++)
        {
            for (int subNum = 1; subNum <= 5; subNum++)
            {
                data.Add(new PlayerStageData
                {
                    Id = userId,
                    StageNum = stageNum,
                    SubStageNum = subNum
                });
            }
        }
        
        await db.ExecuteAsync(sql, data);
    }
}