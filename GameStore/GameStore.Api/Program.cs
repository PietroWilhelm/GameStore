using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using GameStore.API.Exceptions;
using GameStore.API.Extensions;
using GameStore.API.Health;
using GameStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGameStoreDbContext(builder.Configuration, builder.Environment);
builder.Services.AddGameStoreRepositories();
builder.Services.AddGameStoreApplicationServices();
builder.Services.AddGameStoreHealthChecks();

// Converte qualquer exceção não tratada em application/problem+json (GlobalExceptionHandler),
// com fallback padrão do framework para o que não for coberto por ele.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

// Versionamento HTTP: apenas GameController declara versões (1.0 obsoleta / 2.0 atual);
// os demais controllers são [ApiVersionNeutral] e continuam respondendo normalmente.
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(2.0);
        options.AssumeDefaultVersionWhenUnspecified = true;

        // Emite os headers "api-supported-versions" e "api-deprecated-versions" em toda resposta.
        options.ReportApiVersions = true;

        // Suporta query string (?api-version=1.0) e header (X-Api-Version: 1.0); o segmento de
        // URL (api/v1/game, api/v2/game)
        options.ApiVersionReader = ApiVersionReader.Combine(
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Api-Version"),
            new UrlSegmentApiVersionReader());
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        // Formata o nome do grupo como "v1", "v2"
        options.GroupNameFormat = "'v'VVV";

        options.SubstituteApiVersionInUrl = true;
    });

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Política obrigatória: aplicada ao POST /api/game (escrita). 10 requisições/minuto por IP.
    options.AddPolicy("write-fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    // Política extra:aplicada apenas à listagem paginada v2.
    options.AddPolicy("read-fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        var retryAfterSeconds = 60;
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            retryAfterSeconds = (int)retryAfter.TotalSeconds;

        context.HttpContext.Response.Headers.RetryAfter = retryAfterSeconds.ToString();
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/problem+json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                type = "about:blank",
                title = "Limite de requisições excedido",
                status = StatusCodes.Status429TooManyRequests,
                detail = $"Você excedeu o limite de requisições para este endpoint. " +
                         $"Tente novamente em {retryAfterSeconds} segundo(s).",
                instance = context.HttpContext.Request.Path.Value
            },
            cancellationToken);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGameStoreSwagger(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
    db.Database.Migrate();
}

// Registrado cedo no pipeline para cobrir qualquer exceção lançada pelos middlewares seguintes.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Um endpoint Swagger por versão descoberta (v1 obsoleta, v2 atual), lidas via
        // IApiVersionDescriptionProvider — evita hardcodar os nomes dos grupos.
        var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            var label = description.GroupName.ToUpperInvariant() +
                        (description.IsDeprecated ? " (Deprecated)" : "");

            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"GameStore API {label}");
        }

        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

// Único endpoint de health check: processo (self) + banco Oracle + site externo da FIAP.
// Healthy/Degraded -> 200 (ainda serve tráfego); Unhealthy -> 503.
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();
