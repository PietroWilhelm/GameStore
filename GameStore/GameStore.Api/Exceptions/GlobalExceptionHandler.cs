using System.Data.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GameStore.Application.Exceptions;
using GameStore.Domain.Exceptions;

namespace GameStore.API.Exceptions;

/// <summary>
/// Converte qualquer exceção não tratada em uma resposta <c>application/problem+json</c> consistente,
/// evitando <c>try/catch</c> espalhado pelos controllers.
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Exceção não tratada: {Message} : TraceId {TraceId}", exception.Message, traceId);

        var (statusCode, title, detail) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "about:blank",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (environment.IsDevelopment())
        {
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;
        }

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string? Detail) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Requisição inválida", e.Message),
            CreateException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Não foi possível concluir a operação", e.Message),
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Recurso não encontrado", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Não autorizado", e.Message),
            DbException e => (StatusCodes.Status502BadGateway, "Banco indisponível", e.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };
    }
}
