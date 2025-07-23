using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataService
{
    Task<string> AddPlayerDataAsync(string playerName, string sessionId);
    Task<string?> GetPlayerNameByUserIdAsync(int userId, string sessionId);
    Task<int> GetPlayerLevelByUserIdAsync(int userId, string sessionId);
    Task<int> GetPlayerExpByUserIdAsync(int userId, string sessionId);
    Task<PlayerGameData> UpdatePlayerLevelAsync(int userId, int level, int exp, string sessionId);
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId, string sessionId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data, string sessionId);
    Task RenamePlayerNameAsync(int userId, string newPlayerName, string sessionId);
}