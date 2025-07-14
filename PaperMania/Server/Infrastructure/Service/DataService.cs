using Server.Application.Port;

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
    
    public async Task<string> AddPlayerNameAsync(string playerName, string sessionId)
    {
        var isVaild = await _sessionService.ValidateSessionAsync(sessionId);
        if (!isVaild)
            throw new UnauthorizedAccessException("세션이 유효하지 않습니다.");
        
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
        
        await _dataRepository.AddPlayerNameAsync(playerName);
        await _accountRepository.UpdateIsNewAccountAsync(userId, false);
        
        return playerName;
    }
}