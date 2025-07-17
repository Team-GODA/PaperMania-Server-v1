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
            SELECT P.user_id AS Id, P.character_id AS CharacterId, P.character_level AS CharacterLevel,
                   P.normal_skill_level AS NormalSkillLevel, P.epic_skill_level AS EpicSkillLevel,
                   C.character_name AS CharacterName, C.rarity AS RarityString
            FROM player_character_data P
            JOIN character_data C ON P.character_id = C.character_id
            WHERE P.user_id = @Id
            ";

        var result = (await _db.QueryAsync<PlayerCharacterData>(sql, new { Id = userId })).ToList();
        return result;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(int userId, PlayerCharacterData data)
    {
        var sql = @"
            INSERT INTO player_character_data 
            (user_id, character_id, character_name, character_level, normal_skill_level, epic_skill_level, rarity)
            VALUES 
            (@UserId, @CharacterId, @CharacterName, @CharacterLevel, @NormalSkillLevel, @EpicSkillLevel, @RarityString);
            ";
        
        var param = new
        {
            data.CharacterId,
            data.CharacterName,
            data.CharacterLevel,
            data.NormalSkillLevel,
            data.EpicSkillLevel,
            data.RarityString,
            data.Id,
            UserId = userId
        };
        
        await _db.ExecuteAsync(sql, param);

        var result = await _db.QuerySingleAsync<PlayerCharacterData>(
            "SELECT * FROM player_character_data WHERE id = @Id AND user_id = @UserId",
            new { data.Id, UserId = userId });
        
        return result;
    }
}