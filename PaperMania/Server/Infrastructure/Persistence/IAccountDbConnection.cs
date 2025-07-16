using System.Data;

namespace Server.Infrastructure.Persistence;

public interface IAccountDbConnection : IDbConnection
{
    Task OpenAsync(CancellationToken cancellationToken = default);
}