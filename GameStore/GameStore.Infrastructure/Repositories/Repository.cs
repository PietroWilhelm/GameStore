using GameStore.Application.Repositories;
using GameStore.Domain.Common;
using GameStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly GameStoreContext Context;
    protected readonly DbSet<T> DbSet;

    protected Repository(GameStoreContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public IReadOnlyList<T> GetAll()
        => DbSet.AsNoTracking().OrderBy(x => x.Id).ToList();

    public T? GetById(Guid id)
        => DbSet.FirstOrDefault(x => x.Id == id);

    public T Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        DbSet.Add(entity);
        try
        {
            Context.SaveChanges();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            // surface inner exception message to help debugging (preserve original exception)
            var inner = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"An error occurred while saving the entity changes: {inner}", ex);
        }

        return entity;
    }

    public bool Delete(Guid id)
    {
        var entity = DbSet.FirstOrDefault(x => x.Id == id);
        if (entity is null)
            return false;

        DbSet.Remove(entity);
        Context.SaveChanges();
        return true;
    }

    public bool ExistsById(Guid id)
        => DbSet.Any(x => x.Id == id);
}
