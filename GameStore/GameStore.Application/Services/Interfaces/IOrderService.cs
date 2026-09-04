using GameStore.Application.DTOs;

namespace GameStore.Application.Services.Interfaces;

/// <summary>
/// Casos de uso de Order (camada de aplicação).
/// </summary>
public interface IOrderService
{
    IReadOnlyList<OrderResponse> GetAll();

    IReadOnlyList<OrderResponse> GetByCustomer(Guid customerId);

    OrderResponse? GetById(Guid id);

    OrderResponse Create(OrderRequest request);

    bool Delete(Guid id);
}
