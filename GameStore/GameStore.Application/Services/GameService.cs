using GameStore.Application.DTOs;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services;

/// <summary>
/// Implementação dos serviços de negócio relacionados a jogos.
/// Responsável por criar, listar, buscar e remover Games
/// </summary>
public class GameService : IGameService
{
    private readonly List<Game> _games;
    
    /// <inheritdoc />
    public IReadOnlyList<GameReponse> GetAll()
    {   
        //Select g.* from games g
        return _games.Select(GameReponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public GameReponse? GetById(Guid id)
    {
        //Select g.* from game g where id = _id
        var game = _games.Find(g => g.Id == id);
        
        return game is null ? null : GameReponse.FromDomain(game);
    }

    /// <inheritdoc />
    public GameReponse Create(GameRequest request)
    {
       if(ExistsByName(request.Name))
           throw new InvalidOperationException("Já existe um jogo com esse nome.");
       
       var game = request.toDomain();

       // insert into games
       _games.Add(game);
        
       return GameReponse.FromDomain(game);
    }
    
    /// <inheritdoc />
    public bool ExistsByName(string name)
    {
        // select g.* from g where uppercase(g.name) = uppercase(_name)
        return _games.Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc />
    public bool ExistsById(Guid id)
    {
        // select g.* from g where id == _id
        return _games.Any(g => g.Id == id);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        //DELETE FROM GAMES where id = _id
        return _games.RemoveAll(g => g.Id == id) > 0;
    }
}