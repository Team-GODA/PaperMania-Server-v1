namespace Server.Infrastructure.Service.Interface;

public interface ISessionService
{
    Task<string> CreateSessionAsync(int userId);
    Task<bool> ValidateSessionAsync(string sessionId);
    Task<int?> GetUserIdBySessionAsync(string sessionId);
    Task DeleteSessionAsync(string sessionId);
}