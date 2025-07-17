using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task AddPlayerNameAsync(string playerName);
    Task<PlayerGameData?> GetPlayerDataByIdAsync(int userId);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int userId, int newLevel, int newExp);
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(int userId, PlayerCharacterData data);
}