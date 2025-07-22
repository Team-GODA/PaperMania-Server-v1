using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class DataService : IDataService
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<DataService> _logger;

    public DataService(IDataRepository dataRepository, IAccountRepository accountRepository, ISessionService sessionService, ILogger<DataService> logger)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
        _sessionService = sessionService;
        _logger = logger;
    }
    
    public async Task<string> AddPlayerDataAsync(string playerName, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
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
        await _dataRepository.AddPlayerGoodsDataByUserIdAsync(userId);
        await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }

    public async Task<string?> GetPlayerNameByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await GetByPlayerByIdAsync(userId);

        return data?.PlayerName;
    }

    public async Task<PlayerGameData?> GetByPlayerByIdAsync(int userId)
    {
        return await _dataRepository.GetPlayerDataByIdAsync(userId);
    }

    public async Task<int> GetPlayerLevelByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await GetPlayerDataByUserId(userId);

        return data.PlayerLevel;
    }

    public async Task<int> GetPlayerExpByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await GetPlayerDataByUserId(userId);

        return data.PlayerExp;
    }

    public async Task<PlayerGameData> UpdatePlayerLevelAsync(int userId, int level, int exp, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await _dataRepository.UpdatePlayerLevelAsync(userId, level, exp);
        if (data == null)
            throw new Exception($"Id: {userId}의 플레이어 레벨 데이터가 없습니다.");

        return data;
    }

    public async Task<IEnumerable<PlayerCharacterData>> GetPlayerCharacterDataByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        return await _dataRepository.GetPlayerCharacterDataByUserIdAsync(userId);
    }

    public async Task<PlayerCharacterData> AddPlayerCharacterDataByUserIdAsync(PlayerCharacterData data, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        bool exists = await _dataRepository.IsNewCharacterExistAsync(data.Id, data.CharacterId);
        if (exists)
            throw new InvalidOperationException("이미 해당 캐릭터를 보유 중입니다.");
        
        return await _dataRepository.AddPlayerCharacterDataByUserIdAsync(data);
    }

    public async Task RenamePlayerNameAsync(int userId, string newPlayerName, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        
        var exists = await _dataRepository.ExistsPlayerNameAsync(newPlayerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {newPlayerName}");
            throw new InvalidOperationException("이미 존재하는 플레이어 이름입니다.");
        }

        await _dataRepository.RenamePlayerNameAsync(userId, newPlayerName);
    }

    public async Task<PlayerGoodsData> GetPlayerGoodsDataByUserIdAsync(int userId, string sessionId)
    {
        await ValidateSessionAsync(sessionId);
        var data = await _dataRepository.GetPlayerGoodsDataByUserIdAsync(userId);

        return data;
    }

    private async Task ValidateSessionAsync(string sessionId)
    {
        var userId = await _sessionService.GetUserIdBySessionIdAsync(sessionId);
        if (userId == null)
            throw new Exception($"세션 ID에 맞는 유저 ID가 없습니다. : Id : {userId}");
        
        var isValid = await _sessionService.ValidateSessionAsync(sessionId, userId.Value);
        if (!isValid)
            throw new UnauthorizedAccessException("세션이 유효하지 않습니다.");
    }

    private async Task<PlayerGameData> GetPlayerDataByUserId(int userId)
    {
        var data = await _dataRepository.GetPlayerDataByIdAsync(userId);
        if (data == null)
            throw new Exception($"Id: {userId}의 플레이어 데이터가 없습니다.");

        return data;
    }
}