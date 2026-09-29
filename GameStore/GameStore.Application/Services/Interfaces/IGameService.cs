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

    /// <summary>
    /// Lista games de forma paginada (contrato v2.0). Valida o intervalo de <paramref name="page"/>
    /// e <paramref name="pageSize"/> antes de delegar ao repositório.
    /// </summary>
    /// <param name="page">Página solicitada (1-based, mínimo 1).</param>
    /// <param name="pageSize">Itens por página (entre 1 e 100).</param>
    /// <exception cref="ArgumentException">
    /// Quando <paramref name="page"/> ou <paramref name="pageSize"/> estão fora do intervalo permitido.
    /// </exception>
    PagedResponse<GameResponse> GetPaged(int page, int pageSize);
}
