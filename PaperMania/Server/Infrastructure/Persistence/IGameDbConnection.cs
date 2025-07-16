using System.Data;

namespace Server.Infrastructure.Persistence;

public interface IGameDbConnection : IDbConnection
{
    Task OpenAsync(CancellationToken cancellationToken = default);
}