using Server.Application.Port;
using Server.Domain.Entity;

namespace Server.Infrastructure.Service;

public class PlayerService : IPlayerService
{
    private readonly IDataRepository _dataRepository;
    private readonly IAccountRepository _accountRepository;

    public PlayerService(IDataRepository dataRepository, IAccountRepository accountRepository)
    {
        _dataRepository = dataRepository;
        _accountRepository = accountRepository;
    }
    
    public async Task<PlayerAccountData> CreatePlayerAsync(string playerId, string email, string password, string playerName)
    {
        var account = new PlayerAccountData
        {
            PlayerId = playerId,
            Email = email,
            Password = password,
            IsNewAccount = true,
            Role = "user"
        };
        
        var createdAccount = await _accountRepository.AddAccountAsync(account);
        var userId = createdAccount.Id;

        await _dataRepository.AddPlayerDataAsync(userId, playerName);
        return createdAccount;
    }
}