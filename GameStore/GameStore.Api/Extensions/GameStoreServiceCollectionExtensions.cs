using GameStore.Application.Repositories;
using GameStore.Application.Services.Implementations;
using GameStore.Application.Services.Interfaces;
using GameStore.Infrastructure.Persistence;
using GameStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Extensions;

public static class GameStoreServiceCollectionExtensions
{
    public static IServiceCollection AddGameStoreDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment env)
    {
        var connectionString = configuration.GetConnectionString("GameStoreOracle");

        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("OracleConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'GameStoreOracle' (ou 'OracleConnection') não encontrada ou vazia. " +
                "Configure com 'dotnet user-secrets set ConnectionStrings:GameStoreOracle \"...\"' " +
                "(a partir da pasta GameStore.Api) ou em appsettings.Development.json.");
        }

        services.AddDbContext<GameStoreContext>(options =>
        {
            options.UseOracle(connectionString, oracleOptions =>
            {
                // O provider Oracle.EntityFrameworkCore, por padrão, gera literais booleanos
                // ANSI (TRUE/FALSE) em traduções como .Any(), o que só existe nativamente a
                // partir do Oracle 23ai (onde bool vira o tipo BOOLEAN nativo). Em versões
                // anteriores (19c, 21c etc., como a do FIAP) isso quebra com
                // "ORA-00904: TRUE/FALSE: identificador inválido". DatabaseVersion19 faz o
                // provider mapear bool para NUMBER(1) (que já é como a coluna Active está
                // mapeada no banco) e gerar CASE ... THEN 1 ELSE 0 em vez de TRUE/FALSE.
                oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19);
            });

            if (env.IsDevelopment())
            {
                // Log SQL and detailed EF messages in Development to surface inner Oracle errors
                options.LogTo(Console.WriteLine, LogLevel.Information);
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    /// <summary>
    /// Registra todas as implementações de repositório como <c>Scoped</c> (um por requisição HTTP).
    /// </summary>
    public static IServiceCollection AddGameStoreRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    /// <summary>
    /// Registra os serviços de aplicação que orquestram os repositórios.
    /// </summary>
    public static IServiceCollection AddGameStoreApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IStudioService, StudioService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
