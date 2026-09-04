using Microsoft.Extensions.Diagnostics.HealthChecks;
using GameStore.Infrastructure.Persistence;

namespace GameStore.API.Health;

/// <summary>
/// Verifica se o banco Oracle configurado está acessível, sem depender de um pacote
/// de health check específico de provider.
/// </summary>
public sealed class GameStoreDbHealthCheck(GameStoreContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            return canConnect
                ? HealthCheckResult.Healthy("Banco de dados acessível.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao conectar ao banco de dados.", ex);
        }
    }
}
