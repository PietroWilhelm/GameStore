using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia as desenvolvedoras de jogos cadastradas no sistema.
/// </summary>
// Recurso não versionado
[ApiVersionNeutral]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class StudioController(IStudioService studioService) : ControllerBase
{
    /// <summary>
    /// Lista todas as desenvolvedoras cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StudioResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(studioService.GetAll());

    /// <summary>
    /// Busca uma desenvolvedora pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var studio = studioService.GetById(id);
        return studio is null ? NotFound() : Ok(studio);
    }

    /// <summary>
    /// Cria uma nova desenvolvedora.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(StudioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] StudioRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = studioService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Remove uma desenvolvedora pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => studioService.Delete(id) ? NoContent() : NotFound();
}
