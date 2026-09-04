using GameStore.API.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.API.Extensions;

/// <summary>
/// Registra os health checks da API, mantendo o <c>Program.cs</c> enxuto.
/// </summary>
public static class HealthCheckServiceCollectionExtensions
{
    public static IServiceCollection AddGameStoreHealthChecks(this IServiceCollection services)
    {
        // HttpClient nomeado usado pelo check de dependência externa (site da FIAP).
        services.AddHttpClient(nameof(FiapSiteHealthCheck));

        services.AddHealthChecks()
            // Processo no ar: não depende de nenhuma dependência externa.
            .AddCheck(
                "self",
                () => HealthCheckResult.Healthy("O processo da API está no ar."),
                tags: ["live"])
            // Banco Oracle usado pela API (via GameStoreContext.Database.CanConnectAsync).
            .AddCheck<GameStoreDbHealthCheck>(
                "oracle",
                tags: ["ready", "db"])
            // Dependência externa opcional (recomendado no enunciado do CP4).
            .AddCheck<FiapSiteHealthCheck>(
                "fiap-site",
                tags: ["ready", "external"]);

        return services;
    }
}
