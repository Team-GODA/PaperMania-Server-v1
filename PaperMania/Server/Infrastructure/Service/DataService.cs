using Server.Application.Port;

namespace Server.Infrastructure.Service;

public class DataService : IDataService
{
    private readonly IDataRepository _repository;
    private readonly ILogger<DataService> _logger;

    public DataService(IDataRepository repository, ILogger<DataService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<string> AddPlayerNameAsync(string playerName)
    {
        var exists = await _repository.ExistsPlayerNameAsync(playerName);
        if (exists != null)
        {
            _logger.LogWarning($"이미 존재하는 이름입니다. player_name: {playerName}");
            throw new InvalidOperationException("이미 존재하는 플레이어 이름입니다.");
        }
        
        var userId = await _repository.GetUserIdByPlayerNameAsync(playerName);
        
        var isNewAccount = await _repository.IsNewAccountAsync(userId);
        if (!isNewAccount)
        {
            _logger.LogWarning($"이미 이름을 등록한 계정입니다. player_name: {playerName}");
            throw new InvalidOperationException("<이미 이름을 등록한 계정입니다.");
        }
        
        await _repository.AddPlayerNameAsync(playerName);
        await _repository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }

    public async Task<bool> IsPlayerNewAccount(string playerName)
    {
        var account = await _repository.ExistsPlayerNameAsync(playerName);
        return account == null;
    }
}