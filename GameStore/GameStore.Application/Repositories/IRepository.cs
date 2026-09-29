using GameStore.Domain.Common;

namespace GameStore.Application.Repositories;

/// <summary>
/// Contrato genérico de persistência para entidades que derivam de <see cref="BaseEntity"/>.
/// </summary>
/// <typeparam name="T">Tipo da entidade de domínio.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    IReadOnlyList<T> GetAll();

    T? GetById(Guid id);

    T Add(T entity);

    bool Delete(Guid id);

    bool ExistsById(Guid id);

    /// <summary>
    /// Retorna uma página de resultados ordenada por <c>CreatedAt</c> (ordenação obrigatória para
    /// páginas reprodutíveis), junto com o total de itens existentes. O corte (Skip/Take) e a
    /// contagem são executados no banco, sobre <see cref="IQueryable{T}"/> — nunca em memória
    /// após um <c>ToList()</c>/<c>GetAll()</c>.
    /// </summary>
    /// <param name="page">Página solicitada (1-based).</param>
    /// <param name="pageSize">Quantidade de itens por página.</param>
    /// <returns>Os itens da página solicitada e o total de itens existentes.</returns>
    (IReadOnlyList<T> Items, int TotalItems) GetPaged(int page, int pageSize);
}
