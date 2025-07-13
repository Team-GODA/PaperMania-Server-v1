using Server.Application.Port;
using Server.Domain.Entity;
using Server.Infrastructure.Service.Interface;

namespace Server.Infrastructure.Service;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ISessionService _sessionService;

    public AccountService(IAccountRepository repository, ISessionService sessionService)
    {
        _repository = repository;
        _sessionService = sessionService;
    }
    
    public async Task<PlayerAccountData?> GetByPlayerIdAsync(string playerId)
    {
        return await _repository.GetByPlayerIdAsync(playerId);
    }

    public async Task<PlayerAccountData?> GetByEmailAsync(string email)
    {
        return await _repository.GetByEmailAsync(email);
    }

    public async Task RegisterAsync(PlayerAccountData player, string password)
    {
        player.Password = BCrypt.Net.BCrypt.HashPassword(password);
        player.IsNewAccount = true;
        player.Role = "user";
        
        await _repository.AddAccountAsync(player);
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

    public async Task UpdateLastLoginAsync(int playerId)
    {
        throw new NotImplementedException();
    }
}