using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IPlayerService
{
    Task<PlayerAccountData> CreatePlayerAsync(string playerId, string email, string password, string playerName);
}