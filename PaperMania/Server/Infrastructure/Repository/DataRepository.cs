﻿using System.Data;
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

    public async Task<LevelDefinition?> GetLevelDataAsync(int currentLevel)
    {
        await using var db = CreateConnection();
        await db.OpenAsync();

        var sql = @"
            SELECT level AS Level, max_exp AS MaxExp, max_action_point AS MaxActionPoint
            FROM paper_mania_game_data.level_definition
            WHERE level = @CurrentLevel";
        
        return await db.QueryFirstOrDefaultAsync<LevelDefinition>(sql, new { CurrentLevel = currentLevel });
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
}