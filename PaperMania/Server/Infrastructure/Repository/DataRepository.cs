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

    public async Task AddPlayerNameAsync(string playerName)
    {
        var sql = @"
            INSERT INTO player_game_data (player_name)
            VALUES (@PlayerName)";

        await _db.ExecuteAsync(sql, new { PlayerName = playerName });
    }

    public async Task<PlayerGameData?> GetPlayerDataByIdAsync(int userId)
    {
        var sql = @"
            SELECT id AS Id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM player_game_data
            WHERE id = @Id
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { Id = userId });
    }

    public async Task<PlayerGameData?> UpdatePlayerLevelAsync(int userId, int newLevel, int newExp)
    {
        var sql = @"
            UPDATE player_game_data
            SET player_level = @Level, player_exp = @Exp
            WHERE id = @Id
            RETURNING id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel;
            ";

        return await _db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new
        {
            Level = newLevel,
            Exp = newExp,
            Id = userId
        });
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId)
    {
        var sql = @"
            SELECT P.id AS Id, P.character_id AS CharacterId, P.character_level AS CharacterLevel,
                   C.character_name AS CharacterName, C.rarity  AS RarityString
            FROM player_character_data P
            JOIN character_data C ON P.character_id = C.character_id
            WHERE P.id = @Id
            ";

        var result = await _db.QueryAsync<PlayerCharacterData>(sql, new { Id = userId });
        return result;
    }
}