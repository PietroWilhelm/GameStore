using GameStore.Domain.Entities;

namespace GameStore.Application.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Customer? GetByEmail(string email);
    Customer? GetByCpf(string cpf);
}
