using System.Data;
using System.Formats.Asn1;
using Dapper;
using Npgsql;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class DataRepository : RepositoryBase, IDataRepository
{
    public DataRepository(string connectionString) : base(connectionString)
    {
    }
    
    public async Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE player_name = @PlayerName
            LIMIT 1";
        
        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { PlayerName = playerName });
    }

    public async Task AddPlayerDataAsync(int? userId, string playerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
        INSERT INTO paper_mania_game_data.player_game_data (id, player_name)
        VALUES (@UserId, @PlayerName)";

        await db.ExecuteAsync(sql, new { UserId = userId, PlayerName = playerName });
    }

    public async Task<PlayerGameData?> GetPlayerDataByIdAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT id AS Id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel
            FROM paper_mania_game_data.player_game_data
            WHERE id = @Id
            LIMIT 1";
        
        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new { Id = userId });
    }

    public async Task<PlayerGameData?> UpdatePlayerLevelAsync(int userId, int newLevel, int newExp)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_level = @Level, player_exp = @Exp
            WHERE id = @Id
            RETURNING id, player_name AS PlayerName, player_exp AS PlayerExp, player_level AS PlayerLevel;
            ";

        return await db.QueryFirstOrDefaultAsync<PlayerGameData>(sql, new
        {
            Level = newLevel,
            Exp = newExp,
            Id = userId
        });
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT P.user_id AS Id, P.character_id AS CharacterId, P.character_level AS CharacterLevel,
                   P.normal_skill_level AS NormalSkillLevel, P.epic_skill_level AS EpicSkillLevel,
                   C.character_name AS CharacterName, C.rarity AS RarityString
            FROM paper_mania_game_data.player_character_data P
            JOIN paper_mania_game_data.character_data C ON P.character_id = C.character_id
            WHERE P.user_id = @Id
            ";

        var result = (await db.QueryAsync<PlayerCharacterData>(sql, new { Id = userId })).ToList();
        return result;
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            INSERT INTO paper_mania_game_data.player_character_data (user_id, character_id)
            VALUES (@UserId, @CharacterId);
            ";
        
        var param = new
        {
            UserId = data.Id,
            CharacterId  = data.CharacterId
        };
        
        await db.ExecuteAsync(sql, param);

        var result = await db.QuerySingleAsync<PlayerCharacterData>(@"
                SELECT 
                    P.user_id AS Id,
                    P.character_id AS CharacterId,
                    P.character_level AS CharacterLevel,
                    P.normal_skill_level AS NormalSkillLevel,
                    P.epic_skill_level AS EpicSkillLevel,
                    C.character_name AS CharacterName,
                    C.rarity AS RarityString
                FROM paper_mania_game_data.player_character_data P
                JOIN paper_mania_game_data.character_data C ON P.character_id = C.character_id
                WHERE P.user_id = @UserId AND P.character_id = @CharacterId",
            new { UserId =data.Id, data.CharacterId });
        
        return result;
    }

    public async Task<bool> IsNewCharacterExistAsync(int userId, string characterId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT 1
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @UserId AND character_id = @CharacterId
            LIMIT 1;
    ";

        var result = await db.QueryFirstOrDefaultAsync<int?>(sql, new { UserId = userId, CharacterId = characterId });
        return result.HasValue;
    }

    public async Task RenamePlayerNameAsync(int userId, string newPlayerName)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            UPDATE paper_mania_game_data.player_game_data
            SET player_name = @PlayerName
            WHERE id = @Id
            RETURNING player_name AS PlayerName";
        
        await db.ExecuteAsync(sql, new { PlayerName = newPlayerName, Id = userId });
    }

    public async Task<PlayerCharacterData?> GetCharacterByUserIdAsync(int userId)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();
        
        var sql = @"
            SELECT user_id AS Id, character_name AS CharacterName, rarity AS RarityString
            FROM paper_mania_game_data.player_character_data
            WHERE user_id = @Id
            ";

        return await db.QuerySingleOrDefaultAsync<PlayerCharacterData>(sql, new { Id = userId });
    }
}