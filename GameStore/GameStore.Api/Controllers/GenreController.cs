using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os gêneros dos jogos disponíveis no catálogo.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class GenreController(IGenreRepository genreRepository) : ControllerBase
{
    /// <summary>
    /// Lista todos os gêneros cadastrados.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Genre>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(genreRepository.GetAll());

    /// <summary>
    /// Busca um gênero pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var genre = genreRepository.GetById(id);
        return genre is null ? NotFound() : Ok(genre);
    }

    /// <summary>
    /// Cria um novo gênero.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] GenreRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var genre = new Genre(request.Name, request.Description);
            return Ok(genreRepository.Add(genre));
        }
        catch (InvalidOperationException ex)
        {
            // EF inner exception surfaced in repository - include safe message
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Problem(detail: "An unexpected error occurred while creating the genre.", statusCode: 500);
        }
    }

    /// <summary>
    /// Remove um gênero pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => genreRepository.Delete(id) ? NoContent() : NotFound();
}
