using System.Data;
using Dapper;
using Npgsql;
using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Persistence;

namespace Server.Infrastructure.Repository;

public class AccountRepository : RepositoryBase, IAccountRepository
{
    public AccountRepository(IAccountDbConnection db, ILogger<AccountRepository> logger)
        : base(db, logger)
    {
    }

    public async Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId)
    {
        await CheckConnectionOpenAsync();
        
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
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT id, player_id AS PlayerId, email, password, is_new_account AS IsNewAccount,
                   role AS Role, created_at AS CreatedAt, last_login AS LastLogin
            FROM player_account_data
            WHERE email = @Email
            LIMIT 1";
        
        return await _db.QueryFirstOrDefaultAsync<PlayerAccountData>(sql, new { Email = email });
    }

    public async Task AddAccountAsync(PlayerAccountData player)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            INSERT INTO player_account_data (player_id, email, password, is_new_account, role)
            VALUES (@PlayerId, @Email, @Password, @IsNewAccount, @Role)";
    
        await _db.ExecuteAsync(sql, player);
    }

    public async Task UpdateLastLoginAsync(int playerId)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"UPDATE player_account_data SET last_login = NOW() WHERE id = @Id";
        await _db.ExecuteAsync(sql, new { Id = playerId });
    }
    
    public async Task<bool> IsNewAccountAsync(int? userId)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            SELECT is_new_account AS IsNewAccount
            FROM player_account_data
            WHERE Id = @Id
            LIMIT 1";
        
        return await _db.ExecuteScalarAsync<bool>(sql, new { Id = userId });
    }
    
    public async Task UpdateIsNewAccountAsync(int? userId, bool isNew = true)
    {
        await CheckConnectionOpenAsync();
        
        var sql = @"
            UPDATE player_account_data
            SET is_new_account = @IsNew
            WHERE id = @Id";

        await _db.ExecuteAsync(sql, new { IsNew = isNew, Id = userId });
    }
}