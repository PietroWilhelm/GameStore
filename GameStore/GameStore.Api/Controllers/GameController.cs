using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Controller responsável pelas operações de Games na API.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class GameController(IGameService gameService, ILogger<GameController> logger) : ControllerBase
{
    /// <summary>
    /// Lista todos os games cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GameResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(gameService.GetAll());

    /// <summary>
    /// Busca um game pelo identificador único.
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
    /// Cria um novo game.
    /// </summary>
    /// <param name="request">Dados do game a ser criado.</param>
    [HttpPost]
    [ProducesResponseType(typeof(GameResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
    /// Remove um game pelo identificador único.
    /// </summary>
    /// <param name="id">Identificador único do game.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => gameService.Delete(id) ? NoContent() : NotFound();
}
