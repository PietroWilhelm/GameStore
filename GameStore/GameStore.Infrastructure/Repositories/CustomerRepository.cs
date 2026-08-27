using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Persistence;

namespace GameStore.Infrastructure.Repositories;

public class CustomerRepository(GameStoreContext context) : Repository<Customer>(context), ICustomerRepository
{
    public Customer? GetByEmail(string email)
        => Context.customers.FirstOrDefault(x => x.Email == email);

    public Customer? GetByCpf(string cpf)
        => Context.customers.FirstOrDefault(x => x.Cpf == cpf);
}
