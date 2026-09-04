using GameStore.Application.DTOs;

namespace GameStore.Application.Services.Interfaces;

/// <summary>
/// Casos de uso de Genre (camada de aplicação).
/// </summary>
public interface IGenreService
{
    IReadOnlyList<GenreResponse> GetAll();

    GenreResponse? GetById(Guid id);

    GenreResponse Create(GenreRequest request);

    bool Delete(Guid id);
}
