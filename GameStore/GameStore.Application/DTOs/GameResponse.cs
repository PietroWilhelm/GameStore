using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de resposta para Game.
/// </summary>
/// <param name="Id">Identificador único do game.</param>
/// <param name="Name">Nome do game.</param>
public record GameResponse(Guid Id, string Name)
{
    /// <summary>
    /// Cria um GameResponse a partir de uma entidade Game do domínio.
    /// </summary>
    /// <param name="game">Entidade de domínio.</param>
    /// <returns>DTO de resposta.</returns>
    public static GameResponse FromDomain(Game game) => new(game.Id, game.Name);
}
