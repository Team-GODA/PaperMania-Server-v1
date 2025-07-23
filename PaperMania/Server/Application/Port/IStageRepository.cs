namespace Server.Application.Port;

public interface IStageRepository
{
    Task CreatePlayerStageDataAsync(int userId);
}