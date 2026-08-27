using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Persistence;

namespace GameStore.Infrastructure.Repositories;

public class GenreRepository(GameStoreContext context) : Repository<Genre>(context), IGenreRepository
{
}
