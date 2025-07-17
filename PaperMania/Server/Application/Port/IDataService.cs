using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataService
{
    Task<string> AddPlayerNameAsync(string playerName, string sessionId);
    Task<string?> GetPlayerNameByUserIdAsync(int userId, string sessionId);
    Task<PlayerGameData?> GetByPlayerByIdAsync(int userId);
    Task<int> GetPlayerLevelByUserIdAsync(int userId, string sessionId);
    Task<int> GetPlayerExpByUserIdAsync(int userId, string sessionId);
    Task<PlayerGameData> UpdatePlayerLevelAsync(int userId, int level, int exp, string sessionId);
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId, string sessionId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(int userId, PlayerCharacterData data, string sessionId);
}