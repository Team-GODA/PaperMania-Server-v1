using System.Data;
using Dapper;
using Npgsql;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class DataRepository : IDataRepository
{
    private readonly IDbConnection _db;

    public DataRepository(string connectionString)
    {
        _db = new NpgsqlConnection(connectionString);
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        var sql = @"
            SELECT id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { PlayerName = playerName });
    }

    public async Task<bool> IsNewAccountAsync(string playerId)
    {
        var sql = @"
            SELECT is_new_account AS IsNewAccount
            FROM player_account_data
            WHERE player_id = @PlayerId
            LIMIT 1";
        
        return await _db.ExecuteScalarAsync<bool>(sql, new { PlayerId = playerId });
    }

    public async Task AddPlayerNameAsync(string playerName)
    {
        var sql = @"
            INSERT INTO player_game_data (player_name)
            VALUES (@PlayerName)";

        await _db.ExecuteAsync(sql, new { PlayerName = playerName });
    }

    public async Task UpdateIsNewAccountAsync(string playerId, bool isNew = true)
    {
        var sql = @"
            UPDATE player_account_data
            SET is_new_account = @IsNew
            WHERE player_id = @PlayerId";

        await _db.ExecuteAsync(sql, new { IsNew = isNew, PlayerId = playerId });
    }
}