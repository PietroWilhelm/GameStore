using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os gêneros dos jogos disponíveis no catálogo.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class GenreController(IGenreService genreService) : ControllerBase
{
    /// <summary>
    /// Lista todos os gêneros cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GenreResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(genreService.GetAll());

    /// <summary>
    /// Busca um gênero pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var genre = genreService.GetById(id);
        return genre is null ? NotFound() : Ok(genre);
    }

    /// <summary>
    /// Cria um novo gênero.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] GenreRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = genreService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Remove um gênero pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => genreService.Delete(id) ? NoContent() : NotFound();
}
