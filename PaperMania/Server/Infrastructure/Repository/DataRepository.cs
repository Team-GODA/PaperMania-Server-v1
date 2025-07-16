using System.Data;
using Dapper;
using Npgsql;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repository;

public class DataRepository : RepositoryBase, IDataRepository
{
    public DataRepository(IGameDbConnection db, ILogger<DataRepository> logger) 
        : base(db, logger)
    {
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { PlayerName = playerName });
    }

    public async Task AddPlayerNameAsync(string playerName)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            INSERT INTO player_game_data (player_name)
            VALUES (@PlayerName)";

        await _db.ExecuteAsync(sql, new { PlayerName = playerName });
    }

    public async Task<string> GetPlayerNameByUserIdAsync(int userId)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT player_name AS PlayerName
            FROM player_game_data
            WHERE id = @Id
            ";
        
        return await _db.QueryFirstOrDefaultAsync<string>(sql, new { Id = userId });
    }

    public async Task<PlayerGameData?> GetByPlayerByIdAsync(int userId)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT id AS Id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM player_game_data
            WHERE id = @Id
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { Id = userId });
    }

    public async Task<int> GetPlayerLevelByIdAsync(int userId)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT player_level AS PlayerLevel
            FROM player_game_data
            WHERE id = @Id
            ";
        
        return await _db.QueryFirstOrDefaultAsync<int>(sql, new { Id = userId });
    }
}