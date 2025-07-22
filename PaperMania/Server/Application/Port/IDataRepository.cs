using Server.Domain.Entity;

namespace Server.Application.Port;

public interface IDataRepository
{
    Task<PlayerGameData?> ExistsPlayerNameAsync(string playerName);
    Task AddPlayerDataAsync(int? userId, string playerName);
    Task<PlayerGameData?> GetPlayerDataByIdAsync(int userId);
    Task<PlayerGameData?> UpdatePlayerLevelAsync(int userId, int newLevel, int newExp);
    Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId);
    Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data);
    Task<bool> IsNewCharacterExistAsync(int userId, string characterId);
    Task RenamePlayerNameAsync(int userId, string newPlayerName);
    Task AddPlayerGoodsDataByUserIdAsync(int? userId);
    Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int userId);
}