using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia as desenvolvedoras de jogos cadastradas no sistema.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StudioController(IStudioRepository studioRepository) : ControllerBase
{
    /// <summary>
    /// Lista todas as desenvolvedoras cadastradas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Studio>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(studioRepository.GetAll());

    /// <summary>
    /// Busca uma desenvolvedora pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Studio), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var studio = studioRepository.GetById(id);
        return studio is null ? NotFound() : Ok(studio);
    }

    /// <summary>
    /// Cria uma nova desenvolvedora.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Studio), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] StudioRequest request)
    {
        try
        {
            var studio = new Studio(request.Name, request.FoundationDate);
            return Ok(studioRepository.Add(studio));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove uma desenvolvedora pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => studioRepository.Delete(id) ? NoContent() : NotFound();
}
