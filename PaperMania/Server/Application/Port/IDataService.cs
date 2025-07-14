namespace Server.Application.Port;

public interface IDataService
{
    public Task<string> AddPlayerNameAsync(string playerName);
    public Task<bool> IsPlayerNewAccount(string playerName);
}