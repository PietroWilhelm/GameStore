using GameStore.Application.Repositories;
using GameStore.Application.Services;
using GameStore.Infrastructure;
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
        var connectionString = configuration.GetConnectionString("GameStoreOracle")
            ?? configuration.GetConnectionString("OracleConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'GameStoreOracle' (ou 'OracleConnection') não encontrada. Configure em appsettings.json ou no ambiente.");

        services.AddDbContext<GameStoreContext>(options =>
        {
            options.UseOracle(connectionString);

            if (env.IsDevelopment())
            {
                // Log SQL and detailed EF messages in Development to surface inner Oracle errors
                options.LogTo(Console.WriteLine, LogLevel.Information);
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    public static IServiceCollection AddGameStoreRepositories(this IServiceCollection services)
    {
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    public static IServiceCollection AddGameStoreApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
