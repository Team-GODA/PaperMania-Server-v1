using Npgsql;

namespace Server.Infrastructure.Repository;

public class RepositoryBase
{ 
    private readonly string _connectionString;

    protected RepositoryBase(string connectionString)
    {
        _connectionString = connectionString;
    }
    protected NpgsqlConnection CreateConnection() => new NpgsqlConnection(_connectionString);
}