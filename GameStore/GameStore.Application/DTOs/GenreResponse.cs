using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de resposta para gênero.
/// </summary>
public record GenreResponse(Guid Id, string Name, string Description)
{
    /// <summary>
    /// Mapeia <see cref="Genre"/> para DTO.
    /// </summary>
    public static GenreResponse FromDomain(Genre genre) =>
        new(genre.Id, genre.Name, genre.Description);
}
