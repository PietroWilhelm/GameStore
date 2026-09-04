using Microsoft.Extensions.Logging;
using GameStore.Application.DTOs;
using GameStore.Application.Exceptions;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Interfaces;

namespace GameStore.Application.Services.Implementations;

/// <summary>
/// Orquestra listagem, criação e remoção de games.
/// </summary>
public sealed class GameService(
    IGameRepository gameRepository,
    IStudioRepository studioRepository,
    ILogger<GameService> logger) : IGameService
{
    /// <inheritdoc />
    public IReadOnlyList<GameResponse> GetAll()
    {
        return gameRepository.GetAll().Select(GameResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public GameResponse? GetById(Guid id)
    {
        var game = gameRepository.GetById(id);
        return game is null ? null : GameResponse.FromDomain(game);
    }

    /// <inheritdoc />
    public GameResponse Create(GameRequest request)
    {
        var name = request.Name.Trim();

        if (gameRepository.ExistsByName(name))
            throw new CreateException("Já existe um game com este nome.", logger);

        if (!studioRepository.ExistsById(request.StudioId))
            throw new CreateException("Studio não encontrado.", logger);

        var game = request.ToDomain();

        gameRepository.Add(game);
        return GameResponse.FromDomain(game);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        return gameRepository.Delete(id);
    }
}
