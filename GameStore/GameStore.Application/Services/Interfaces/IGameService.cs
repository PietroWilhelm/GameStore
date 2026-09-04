using GameStore.Application.DTOs;

namespace GameStore.Application.Services.Interfaces;

/// <summary>
/// Casos de uso de Game (camada de aplicação).
/// </summary>
public interface IGameService
{
    IReadOnlyList<GameResponse> GetAll();

    GameResponse? GetById(Guid id);

    GameResponse Create(GameRequest request);

    bool Delete(Guid id);
}
