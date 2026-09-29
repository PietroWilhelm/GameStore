using Asp.Versioning;
using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GameStore.Controllers;

/// <summary>
/// Controller responsável pelas operações de Games na API.
/// Único recurso do CP5 com versionamento HTTP: a v1.0 (obsoleta) preserva a listagem antiga,
/// sem paginação, para não quebrar consumidores existentes; a v2.0 introduz a listagem paginada.
/// Ambas as versões compartilham o mesmo <see cref="IGameService"/> — nenhuma regra de negócio
/// é duplicada entre elas. GetById, Create e Delete não têm <see cref="MapToApiVersionAttribute"/>
/// e por isso respondem em qualquer uma das versões declaradas no controller.
/// </summary>
[ApiVersion("1.0", Deprecated = true)]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class GameController(IGameService gameService, ILogger<GameController> logger) : ControllerBase
{
    /// <summary>
    /// [v1.0 - obsoleta] Lista todos os games cadastrados, sem paginação.
    /// Mantida apenas para compatibilidade com consumidores antigos; utilize a v2.0.
    /// </summary>
    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllV1()
        => Ok(gameService.GetAll());

    /// <summary>
    /// [v2.0] Lista os games cadastrados de forma paginada.
    /// </summary>
    /// <param name="page">Página solicitada (1-based, padrão 1).</param>
    /// <param name="pageSize">Itens por página (entre 1 e 100, padrão 20).</param>
    /// <remarks>
    /// Página fora do intervalo existente retorna 200 com <c>items</c> vazio (nunca 404).
    /// <paramref name="page"/> ou <paramref name="pageSize"/> inválidos retornam 400.
    /// </remarks>
    [HttpGet]
    [MapToApiVersion("2.0")]
    [EnableRateLimiting("read-fixed")]
    [ProducesResponseType(typeof(PagedResponse<GameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetAllV2([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        => Ok(gameService.GetPaged(page, pageSize));

    /// <summary>
    /// Busca um game pelo identificador único. Disponível em ambas as versões.
    /// </summary>
    /// <param name="id">Identificador único do game.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var game = gameService.GetById(id);
        return game is null ? NotFound() : Ok(game);
    }

    /// <summary>
    /// Cria um novo game. Disponível em ambas as versões.
    /// Limitado a 10 requisições por minuto por IP (ver README, seção Rate Limiting).
    /// </summary>
    /// <param name="request">Dados do game a ser criado.</param>
    [HttpPost]
    [EnableRateLimiting("write-fixed")]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public IActionResult Create([FromBody] GameRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var traceId = HttpContext.TraceIdentifier;

        logger.LogInformation(
            "Iniciando criação de game. Nome: {GameName}, StudioId: {StudioId}, TraceId: {TraceId}",
            request.Name, request.StudioId, traceId);

        var created = gameService.Create(request);

        logger.LogInformation(
            "Game criado com sucesso. GameId: {GameId}, Nome: {GameName}, TraceId: {TraceId}",
            created.Id, created.Name, traceId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Remove um game pelo identificador único. Disponível em ambas as versões.
    /// </summary>
    /// <param name="id">Identificador único do game.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => gameService.Delete(id) ? NoContent() : NotFound();
}
