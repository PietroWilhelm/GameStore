using GameStore.Application.DTOs;
using GameStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Controller responsável pelas operações de Games na API.
/// Expõe endpoints para criar, ler, atualizar e deletar jogos, bem como para listar jogos por categoria, buscar jogos por nome e obter detalhes de um jogo específico.
/// </summary>
/// <remarks>
/// Base URL: /api/game
/// Exemplo: http://:localhost:5283/api/game
/// </remarks>

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Iniciliza o controller com o serviço de games
    /// </summary>
    /// <param name="gameservice">Implementação do serviço de game (injetado via DI).</param>
    public GameController(IGameService gameservice)
    {
        _gameService = gameservice;
    }

    /// <summary>
    /// Listando todos os games cadastrados
    /// </summary>
    /// <returns>Lista de games</returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var games = _gameService.GetAll();
        return new JsonResult(games);
    }

    /// <summary>
    /// Busca um game pelo identificador único.
    /// </summary>
    /// <param name="id">Identificador único do game.</param>
    /// <returns>O game encontrado ou 404 se não existir.</returns>
    [HttpGet("{id:guid}")]
    public IActionResult GetbyId(Guid id)
    {
        var game = _gameService.GetById(id);
        if (game is null)
            return NotFound();

        return Ok(game);
    }

    /// <summary>
    /// Cria um novo game.
    /// </summary>
    /// <param name="request">Dados do game a ser criado.</param>
    /// <returns>O game criado ou 400 se o nome já existir.</returns>
    [HttpPost]
    public IActionResult Create([FromBody] GameRequest request)
    {
        try
        {
            var game = _gameService.Create(request);
            return Ok(game);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove um game pelo identificador único.
    /// </summary>
    /// <param name="id">Identificador único do game.</param>
    /// <returns>204 se removido com sucesso ou 404 se não encontrado.</returns>
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        if (!_gameService.Delete(id))
            return NotFound();

        return NoContent();
    }
}