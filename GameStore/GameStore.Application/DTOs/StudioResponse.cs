using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de resposta para desenvolvedora (studio).
/// </summary>
public record StudioResponse(Guid Id, string Name, DateTime FoundationDate)
{
    /// <summary>
    /// Mapeia <see cref="Studio"/> para DTO.
    /// </summary>
    public static StudioResponse FromDomain(Studio studio) =>
        new(studio.Id, studio.Name, studio.FoundationDate);
}
