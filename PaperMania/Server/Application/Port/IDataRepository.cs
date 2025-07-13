using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task<bool> IsNewAccountAsync(string playerId);
    Task AddPlayerNameAsync(string playerName);
    Task UpdateIsNewAccountAsync(string playerId, bool isNew = true);
}