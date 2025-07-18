using Google.Apis.Auth;
using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;
    private readonly ILogger<AccountService> _logger;

    private readonly string _googleClientId =
        "1072960220447-ifi2uau290btfudnu2ol5b82eq1ucp96.apps.googleusercontent.com";

    public AccountService(IAccountRepository repository, ISessionService sessionService, ILogger<AccountService> logger)
    {
        _repository = repository;
        _sessionService = sessionService;
        _logger = logger;
    }
    
    public async Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId)
    {
        return await _repository.GetByPlayerIdAsync(playerId);
    }

    public async Task<PlayerAccountData?> GetByEmailAsync(string email)
    {
        return await _repository.GetByEmailAsync(email);
    }

    public async Task<PlayerAccountData?> RegisterAsync(PlayerAccountData player, string password)
    {
        var exists = await _repository.GetByEmailAsync(player.Email);
        if (exists != null)
            return null;
        
        player.Password = BCrypt.Net.BCrypt.HashPassword(password);
        player.IsNewAccount = true;
        player.Role = "user";
        
        var createdPlayer = await _repository.AddAccountAsync(player);
        return createdPlayer;
    }

    public async Task<string?> LoginAsync(string playerId, string password)
    {
        var user = await _repository.GetByPlayerIdAsync(playerId);
        if (user == null) 
            return string.Empty;
        
        bool isVerified = BCrypt.Net.BCrypt.Verify(password, user.Password);
        if (!isVerified)
            return string.Empty;
        
        await UpdateLastLoginAsync(user.Id);

        var sessionId = await _sessionService.CreateSessionAsync(user.Id);
        
        return sessionId;
    }

    public async Task<bool> LogoutAsync(string sessionId)
    {
        var isVaild = await _sessionService.ValidateSessionAsync(sessionId);
        if (!isVaild)
            return false;
        
        await _sessionService.DeleteSessionAsync(sessionId);
        return true;
    }

    public async Task<string?> LoginByGoogleAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleClientId }
                });
            
            var user = await _repository.GetByEmailAsync(payload.Email);
            if (user == null)
            {
                _logger.LogWarning("");

                user = await _repository.AddAccountAsync(new PlayerAccountData
                {
                    PlayerId = payload.Name,
                    Password = "",
                    Role = "user",
                    Email = payload.Email,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            var sessionId = await _sessionService.CreateSessionAsync(user.Id);
            return sessionId;
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning("구글 토큰 검증 실패: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError("구글 토큰 검증 중 예기치 못한 오류 발생: {Message}", ex.Message);
            return null;
        }
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var account = await _repository.GetByPlayerIdAsync(userId.ToString());
        if (account != null)
        {
            account.LastLogin = DateTime.UtcNow;
            await _repository.UpdateLastLoginAsync(account.Id);
        }
    }
}