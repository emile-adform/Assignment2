using Domain.Interfaces;
using Infrastructure.Clients;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreConnection") ?? throw new ArgumentNullException("Connection string was not found."); ;
        service.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

        //EnsureDatabase.For.PostgresqlDatabase(connectionString);
        //var upgrader = DeployChanges.To
        //        .PostgresqlDatabase(connectionString)
        //        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        //        .LogToNowhere()
        //        .Build();

        //var result = upgrader.PerformUpgrade();

        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        service.AddHostedService<BackgroundCleanupService>();

        service.AddHttpClient();

        service.AddTransient<IExchangeRatesClient, ExchangeRatesClient>();
        service.AddTransient<IExchangeRatesRepository, ExchangeRatesRepository>();
    }
}
