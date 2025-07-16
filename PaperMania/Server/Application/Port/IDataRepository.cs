using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task AddPlayerNameAsync(string playerName);
    Task<PlayerGameData?> GetByPlayerByIdAsync(int userId);
}