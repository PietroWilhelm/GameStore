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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameStore API v1");
        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

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
