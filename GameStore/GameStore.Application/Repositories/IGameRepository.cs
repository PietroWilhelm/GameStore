using GameStore.Domain.Entities;

namespace GameStore.Application.Repositories;

/// <summary>
/// Contrato de persistência específico de <see cref="Game"/> (ISP:
/// consulta por nome só existe onde o agregado tem nome único).
/// </summary>
public interface IGameRepository : IRepository<Game>
{
    /// <summary>
    /// Verifica se já existe um game com o mesmo nome (case-insensitive após trim).
    /// </summary>
    bool ExistsByName(string name);
}
