using System.Data;
using Npgsql;

namespace Server.Infrastructure.Repository;

public class RepositoryBase
{
    protected IDbConnection _db;
    protected readonly ILogger _logger;

    protected RepositoryBase(IDbConnection db, ILogger logger)
    {
        _db = db;
        _logger = logger;
    }

    protected async Task CheckConnectionOpenAsync()
    {
        try
        {
            if (_db.State != ConnectionState.Open && _db is NpgsqlConnection conn)
            {
                await conn.OpenAsync();
                _logger.LogDebug("DB 연결 성공");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Db 연결 실패.");
            throw;
        }
    }
}