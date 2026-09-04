using Microsoft.Extensions.Logging;
using GameStore.Application.DTOs;
using GameStore.Application.Exceptions;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Interfaces;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.Implementations;

/// <summary>
/// Orquestra os casos de uso de Customer.
/// </summary>
public sealed class CustomerService(
    ICustomerRepository customerRepository,
    ILogger<CustomerService> logger) : ICustomerService
{
    /// <inheritdoc />
    public IReadOnlyList<CustomerResponse> GetAll()
    {
        return customerRepository.GetAll().Select(CustomerResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public CustomerResponse? GetById(Guid id)
    {
        var customer = customerRepository.GetById(id);
        return customer is null ? null : CustomerResponse.FromDomain(customer);
    }

    /// <inheritdoc />
    public CustomerResponse Create(CustomerRequest request)
    {
        if (customerRepository.GetByEmail(request.Email) is not null)
            throw new CreateException("Já existe um cliente com este e-mail.", logger);

        if (customerRepository.GetByCpf(request.Cpf) is not null)
            throw new CreateException("Já existe um cliente com este CPF.", logger);

        var customer = new Customer(request.Name, request.Email, request.BirthDate, request.Password, request.Cpf);
        customerRepository.Add(customer);
        return CustomerResponse.FromDomain(customer);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        return customerRepository.Delete(id);
    }
}
