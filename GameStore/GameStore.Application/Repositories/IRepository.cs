using GameStore.Domain.Commom;

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
}