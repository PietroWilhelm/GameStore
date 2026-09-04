using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.API.Health;

/// <summary>
/// Serializa o <see cref="HealthReport"/> em um JSON legível, tanto por humanos quanto
/// por ferramentas de monitoramento: status geral, duração total e a lista de checks
/// (nome, status e duração de cada um). O detalhe da exceção só é incluído em
/// Development, para não vazar stack trace em produção.
/// </summary>
public static class HealthCheckResponseWriter
{
    public static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

        context.Response.ContentType = "application/json";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                durationMs = e.Value.Duration.TotalMilliseconds,
                error = environment.IsDevelopment() ? e.Value.Exception?.Message : null
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }
}
