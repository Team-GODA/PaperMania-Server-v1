using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class DataService : IDataService
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<DataService> _logger;

    public DataService(IDataRepository dataRepository, IAccountRepository accountRepository
        ,ICurrencyRepository currencyRepository, ISessionService sessionService, ILogger<DataService> logger)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _currencyRepository = currencyRepository;
        _sessionService = sessionService;
        _logger = logger;
    }
    
    public async Task<string> AddPlayerDataAsync(string playerName, string sessionId)
    {
        var exists = await _dataRepository.ExistsPlayerNameAsync(playerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {playerName}");
            throw new InvalidOperationException("이미 존재하는 플레이어 이름입니다.");
        }
        
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        
        var isNewAccount = await _accountRepository.IsNewAccountAsync(userId);
        if (!isNewAccount)
        {
            _logger.LogWarning($"이미 이름을 등록한 계정입니다. player_name: {playerName}");
            throw new InvalidOperationException("이미 이름을 등록한 계정입니다.");
        }
        
        await _dataRepository.AddPlayerDataAsync(userId, playerName);
        await _currencyRepository.AddPlayerGoodsDataByUserIdAsync(userId);
        await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }

    public async Task<string?> GetPlayerNameByUserIdAsync(int userId)
    {
        var data = await GetPlayerDataByIdAsync(userId);

        return data?.PlayerName;
    }

    public async Task<PlayerGameData?> GetPlayerDataByIdAsync(int userId)
    {
        return await _dataRepository.GetPlayerDataByIdAsync(userId);
    }

    public async Task<int> GetPlayerLevelByUserIdAsync(int userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        return data.PlayerLevel;
    }

    public async Task<int> GetPlayerExpByUserIdAsync(int userId)
    {
        var data = await GetPlayerDataByUserId(userId);
        return data.PlayerExp;
    }

    public async Task<PlayerGameData> UpdatePlayerLevelByExpAsync(int userId, int exp)
    {
        var playerData = await GetPlayerDataByUserId(userId);
        playerData.PlayerExp += exp;

        while (true)
        {
            var levelData = await _dataRepository.GetLevelDataAsync(playerData.PlayerLevel);

            if (levelData == null || playerData.PlayerExp < levelData.MaxExp)
                break;

            playerData.PlayerExp -= levelData.MaxExp;
            playerData.PlayerLevel++;
        }

        await _dataRepository.UpdatePlayerLevelAsync(userId, playerData.PlayerLevel, playerData.PlayerExp);
        return playerData;
    }

    public async Task RenamePlayerNameAsync(int userId, string newPlayerName)
    {
        var exists = await _dataRepository.ExistsPlayerNameAsync(newPlayerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {newPlayerName}");
            throw new InvalidOperationException("이미 존재하는 플레이어 이름입니다.");
        }

        await _dataRepository.RenamePlayerNameAsync(userId, newPlayerName);
    }

    public async Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int userId)
    {
        var data = await _currencyRepository.GetPlayerGoodsDataByUserIdAsync(userId);
        return data;
    }

    private async Task<PlayerGameData> GetPlayerDataByUserId(int userId)
    {
        var data = await _dataRepository.GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new Exception($"Id: {userId}의 플레이어 데이터가 없습니다.");

        return data;
    }
}