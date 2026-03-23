using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary
/// DTO de resposta para Game
/// </summary>
/// <param name="id">Identificador único do game.</param>
/// <param name="name">nome do game.</param>

public record GameReponse(Guid id, string name)
{
    /// <summary>
    /// Cria um GameResponse a partir de uma entidade Game do domínio.
    /// </summary>
    /// <param name="game">Entidade de domínio.</param>
    /// <returns>DTO de resposta.</returns>
    public static GameReponse FromDomain(Game game) => new(game.Id, game.Name);
}