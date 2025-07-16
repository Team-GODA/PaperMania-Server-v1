using System.Data;
using Npgsql;

namespace Server.Infrastructure.Persistence;

public class GameDbConnection : IGameDbConnection
{
    private readonly NpgsqlConnection _inner;

    public GameDbConnection(string connectionString)
    {
        _inner = new NpgsqlConnection(connectionString);
    }
    
    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_inner.State != ConnectionState.Open)
            await _inner.OpenAsync(cancellationToken);
    }
    
    public string ConnectionString { get => _inner.ConnectionString; set => _inner.ConnectionString = value; }
    public int ConnectionTimeout => _inner.ConnectionTimeout;
    public string Database => _inner.Database;
    public ConnectionState State => _inner.State;
    public string DataSource => _inner.DataSource;
    public string ServerVersion => _inner.ServerVersion;

    public void Open() => _inner.Open();
    public void Close() => _inner.Close();
    public void Dispose() => _inner.Dispose();
    public void ChangeDatabase(string databaseName) => _inner.ChangeDatabase(databaseName);
    public IDbTransaction BeginTransaction() => _inner.BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel il) => _inner.BeginTransaction(il);
    public IDbCommand CreateCommand() => _inner.CreateCommand();
}