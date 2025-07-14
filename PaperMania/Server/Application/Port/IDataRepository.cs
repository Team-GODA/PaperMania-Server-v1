using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task<bool> IsNewAccountAsync(int? userId);
    Task AddPlayerNameAsync(string playerName);
    Task UpdateIsNewAccountAsync(int? userId, bool isNew = true);
}