using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Persistence;

namespace GameStore.Infrastructure.Repositories;

public class OrderRepository(GameStoreContext context) : Repository<Order>(context), IOrderRepository
{
    public IReadOnlyList<Order> GetByCustomer(Guid customerId)
        => Context.Orders
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.OrderDate)
            .ToList();
}
