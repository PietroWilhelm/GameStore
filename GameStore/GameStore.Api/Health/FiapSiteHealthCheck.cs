using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.API.Health;

/// <summary>
/// Check de dependência externa (recomendado pelo enunciado do CP4): verifica se o site
/// da FIAP está respondendo. Uma falha aqui derruba o status agregado de <c>/health</c>
/// para <see cref="HealthStatus.Unhealthy"/>, evidenciando o impacto de uma dependência
/// externa indisponível no relatório completo.
/// </summary>
public sealed class FiapSiteHealthCheck(IHttpClientFactory httpClientFactory) : IHealthCheck
{
    private const string Url = "https://www.fiap.com.br";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = httpClientFactory.CreateClient(nameof(FiapSiteHealthCheck));
            client.Timeout = TimeSpan.FromSeconds(5);

            using var response = await client.GetAsync(
                Url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{Url} respondeu {(int)response.StatusCode}.")
                : HealthCheckResult.Unhealthy($"{Url} respondeu {(int)response.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Não foi possível alcançar {Url}.", ex);
        }
    }
}
