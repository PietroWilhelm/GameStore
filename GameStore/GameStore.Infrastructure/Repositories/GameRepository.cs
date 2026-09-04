using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Persistence;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// Repositório para operações de persistência e consulta de games.
/// </summary>
public sealed class GameRepository(GameStoreContext context) : Repository<Game>(context), IGameRepository
{
    /// <inheritdoc />
    public bool ExistsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalized = name.Trim().ToLowerInvariant();

        return Context.Set<Game>()
            .Any(g => g.Name.ToLower() == normalized);
    }
}
