using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Persistence;

namespace GameStore.Infrastructure.Repositories;

public class StudioRepository(GameStoreContext context) : Repository<Studio>(context), IStudioRepository
{
}
