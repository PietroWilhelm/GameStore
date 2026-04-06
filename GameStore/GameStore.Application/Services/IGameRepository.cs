using GameStore.Application.DTOs;

namespace GameStore.Application.Services;

public interface IGameRepository
{
    /// <summary>
    /// Retorna todos os Games cadastrados.
    /// </summary>
    /// <returns>Lista de Games no formato de resposta</returns>
    IReadOnlyList<GameReponse> GetAll();

    /// <summary>
    /// Retorna um Game específico, a partir do seu Id.
    /// </summary>
    /// <param name="id">Identificador Único do Game</param>
    /// <returns>O game encontrado ou null se não exister</returns>
    GameReponse? GetById(Guid id);
    
    /// <summary>
    /// Cria um novo Game a partir dos dados fornecidos.
    /// </summary>
    /// <param name="request">Dados do Game a ser criado</param>
    /// <returns>O game Criado no formato de resposta</returns>
    /// <exception cref="InvalidOperationException">Quando já existi um Game com o mesmo nome</exception>
    GameReponse Create(GameRequest request);
    
    /// <summary>
    /// Verifica se existe um Game com o nome informado
    /// </summary>
    /// <param name="name">Nome do Game</param>
    /// <returns>True se o Game existe, false caso o contrário</returns>
    bool ExistsByName(string name);
    
    /// <summary>
    /// Verifica se Game existe a partir do seu ID.
    /// </summary>
    /// <param name="id">Identificador único do Game</param>
    /// <returns>True se o Game existe, false caso contrário</returns>
    bool ExistsById(Guid id);

    /// <summary>
    /// Remove um Game pelo indentificador único.
    /// </summary>
    /// <param name="id">Identificador único do Game a ser removido</param>
    /// <returns>True se o Game foi removido, false caso o Game não foi encontrado</returns>
    bool Delete(Guid id);

}