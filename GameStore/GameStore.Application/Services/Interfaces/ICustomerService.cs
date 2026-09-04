using GameStore.Application.DTOs;

namespace GameStore.Application.Services.Interfaces;

/// <summary>
/// Casos de uso de Customer (camada de aplicação).
/// </summary>
public interface ICustomerService
{
    IReadOnlyList<CustomerResponse> GetAll();

    CustomerResponse? GetById(Guid id);

    CustomerResponse Create(CustomerRequest request);

    bool Delete(Guid id);
}
