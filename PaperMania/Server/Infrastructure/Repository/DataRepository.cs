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

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        var sql = @"
            INSERT INTO player_character_data (user_id, character_id)
            VALUES (@UserId, @CharacterId);
            ";
        
        var param = new
        {
            UserId = data.Id,
            data.CharacterId
        };
        
        await _db.ExecuteAsync(sql, param);

        var result = await _db.QuerySingleAsync<PlayerCharacterData>(@"
                SELECT 
                    P.user_id AS Id,
                    P.character_id AS CharacterId,
                    P.character_level AS CharacterLevel,
                    P.normal_skill_level AS NormalSkillLevel,
                    P.epic_skill_level AS EpicSkillLevel,
                    C.character_name AS CharacterName,
                    C.rarity AS RarityString
                FROM player_character_data P
                JOIN character_data C ON P.character_id = C.character_id
                WHERE P.user_id = @UserId AND P.character_id = @CharacterId",
            new { UserId =data.Id, data.CharacterId });
        
        return result;
    }

    public async Task<bool> IsNewCharacterExistAsync(int userId, string characterId)
    {
        var sql = @"
            SELECT 1
            FROM player_character_data
            WHERE user_id = @UserId AND character_id = @CharacterId
            LIMIT 1;
    ";

        var result = await _db.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId, CharacterId = characterId });
        return result.HasValue;
    }

    public async Task<PlayerCharacterData?> GetCharacterByUserIdAsync(int userId)
    {
        const string sql = @"
            SELECT user_id AS Id, character_name AS CharacterName, rarity AS RarityString
            FROM player_character_data
            WHERE user_id = @Id
            ";

        return await _db.QuerySingleOrDefaultAsync<PlayerCharacterData>(sql, new { Id = userId });
    }
}