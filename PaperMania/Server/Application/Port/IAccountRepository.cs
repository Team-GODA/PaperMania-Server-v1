using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IAccountRepository
{
    Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> GetByEmailAsync(string email);
    Task AddAccountAsync(PlayerAccountData player);
    Task UpdateLastLoginAsync(int playerId);
}