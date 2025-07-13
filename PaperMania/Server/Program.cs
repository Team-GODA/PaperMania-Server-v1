using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Server.Application.Port;
using Server.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

var keyVaultName = "papermaniadbconnection";
var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

builder.Services.AddScoped<IAccountRepository>(provider =>
{
    var connectionString = builder.Configuration["AccountDbConnectionString"];
    return new AccountRepository(connectionString!);
});


builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();