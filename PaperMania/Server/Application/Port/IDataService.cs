using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataService
{
    public Task<string> AddPlayerNameAsync(string playerName, string sessionId);
    public Task<string> GetPlayerNameByUserIdAsync(int userId, string sessionId);
    public Task<PlayerGameData?> GetByPlayerByIdAsync(int userId);
}