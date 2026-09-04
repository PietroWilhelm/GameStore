using GameStore.Application.DTOs;

namespace GameStore.Application.Services.Interfaces;

/// <summary>
/// Casos de uso de Studio (camada de aplicação).
/// </summary>
public interface IStudioService
{
    IReadOnlyList<StudioResponse> GetAll();

    StudioResponse? GetById(Guid id);

    StudioResponse Create(StudioRequest request);

    bool Delete(Guid id);
}
