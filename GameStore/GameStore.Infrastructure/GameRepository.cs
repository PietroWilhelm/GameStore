using GameStore.Application.DTOs;
using GameStore.Application.Services;
using GameStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure;


/// <summary>
/// Repositório para operações de persistência e consulta de jogos.
/// </summary>
public sealed class GameRepository(GameStoreContext gameStoreContext) : IGameRepository
{
    /// <inheritdoc />
    public IReadOnlyList<GameReponse> GetAll()
    {
        return gameStoreContext.Games
            .OrderBy(g => g.Name)
            .Select(GameReponse.FromDomain)
            .ToList();
    }

    /// <inheritdoc />
    public GameReponse? GetById(Guid id)
    {
        var game = gameStoreContext.Games
            .FirstOrDefault(g => g.Id == id);

        return game is null ? null : GameReponse.FromDomain(game);
    }

    /// <inheritdoc />
    public GameReponse Create(GameRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("O nome do game é obrigatório");

        if (ExistsByName(request.Name))
            throw new InvalidOperationException("Já existe um game com este nome");

        var game = request.toDomain();

        gameStoreContext.Games.Add(game);
        gameStoreContext.SaveChanges();

        return GameReponse.FromDomain(game);
    }

    /// <inheritdoc />
    public bool ExistsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalizedName = name.Trim().ToLower();

        return gameStoreContext.Games
            .Any(g => g.Name.ToLower() == normalizedName);
    }

    /// <inheritdoc />
    public bool ExistsById(Guid id)
    {
        return gameStoreContext.Games
            .Any(g => g.Id == id);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        var game = gameStoreContext.Games.FirstOrDefault(g => g.Id == id);
        if (game is null)
            return false;

        gameStoreContext.Games.Remove(game);
        gameStoreContext.SaveChanges();

        return true;
    }
}