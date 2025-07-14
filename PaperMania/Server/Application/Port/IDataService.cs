namespace Server.Application.Port;

public interface IDataService
{
    public Task<string> AddPlayerNameAsync(string playerName, string sessionId);
}