using System.Data;
using Azure.Identity;
using Npgsql;
using Server.Api.Middleware;
using Server.Application.Port;
using Server.Infrastructure.Persistence;
using Server.Infrastructure.Repository;
using Server.Infrastructure.Service;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = "papermaniadbconnection";
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDataService, DataService>();

builder.Services.AddScoped<IAccountDbConnection>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config["AccountDbConnection"];
    return new AccountDbConnection(connectionString!);
});
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddScoped<IGameDbConnection>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration["GameDataDbConnectionString"];
        return new GameDbConnection(connectionString!);
    });
builder.Services.AddScoped<IDataRepository, DataRepository>();


builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

var app = builder.Build();

app.UseMiddleware<SessionRefresh>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();