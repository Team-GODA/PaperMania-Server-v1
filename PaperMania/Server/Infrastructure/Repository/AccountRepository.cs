﻿using System.Data;
using Dapper;
using Npgsql;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnection _db;

    public AccountRepository(string connectionString)
    {
        _db = new NpgsqlConnection(connectionString);
    }
    
    public async Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId)
    {
        var sql = @"
            SELECT id, player_id AS PlayerId, email, password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt, last_login AS LastLogin
            FROM player_account_data
            WHERE player_id = @PlayerId
            LIMIT 1";

        return await _db.QueryFirstOrDefaultAsync<PlayerAccountData>(sql, new { PlayerId = playerId });
    }

    public async Task<PlayerAccountData?> GetByEmailAsync(string email)
    {
        var sql = @"
            SELECT id, player_id AS PlayerId, email, password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt, last_login AS LastLogin
            FROM player_account_data
            WHERE email = @Email
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerAccountData>(sql, new { Email = email });
    }

    public async Task<PlayerAccountData?> AddAccountAsync(PlayerAccountData player)
    {
        var sql = @"
            INSERT INTO player_account_data (player_id, email, password, is_new_account, role)
            VALUES (@PlayerId, @Email, @Password, @IsNewAccount, @Role)
            RETURNING id";
    
        var id = await _db.QuerySingleAsync<int>(sql, player);
        player.Id = id;
        return player;
    }

    public async Task UpdateLastLoginAsync(int playerId)
    {
        var sql = @"UPDATE player_account_data SET last_login = NOW() WHERE id = @Id";
        await _db.ExecuteAsync(sql, new { Id = playerId });
    }
    
    public async Task<bool> IsNewAccountAsync(int? userId)
    {
        var sql = @"
            SELECT is_new_account AS IsNewAccount
            FROM player_account_data
            WHERE Id = @Id
            LIMIT 1";
        
        return await _db.ExecuteScalarAsync<bool>(sql, new { Id = userId });
    }
    
    public async Task UpdateIsNewAccountAsync(int? userId, bool isNew = true)
    {
        var sql = @"
            UPDATE player_account_data
            SET is_new_account = @IsNew
            WHERE id = @Id";

        await _db.ExecuteAsync(sql, new { IsNew = isNew, Id = userId });
    }
}