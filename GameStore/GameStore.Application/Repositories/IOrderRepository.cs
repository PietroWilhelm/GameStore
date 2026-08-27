using GameStore.Domain.Entities;

namespace GameStore.Application.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    IReadOnlyList<Order> GetByCustomer(Guid customerId);
}
