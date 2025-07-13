using Server.Domain.Entity;

namespace Server.Infrastructure.Service.Interface;

public interface IAccountService
{
    Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId);
    Task<PlayerAccountData?> GetByEmailAsync(string email);
    Task RegisterAsync(PlayerAccountData player, string password);
    Task<string?> LoginAsync(string playerId, string password);
    Task<bool> LogoutAsync(string sessionId);
    Task UpdateLastLoginAsync(int playerId);
}