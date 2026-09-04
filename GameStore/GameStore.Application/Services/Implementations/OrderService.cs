using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Interfaces;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.Implementations;

/// <summary>
/// Orquestra os casos de uso de Order.
/// </summary>
public sealed class OrderService(IOrderRepository orderRepository) : IOrderService
{
    /// <inheritdoc />
    public IReadOnlyList<OrderResponse> GetAll()
    {
        return orderRepository.GetAll().Select(OrderResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyList<OrderResponse> GetByCustomer(Guid customerId)
    {
        return orderRepository.GetByCustomer(customerId).Select(OrderResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public OrderResponse? GetById(Guid id)
    {
        var order = orderRepository.GetById(id);
        return order is null ? null : OrderResponse.FromDomain(order);
    }

    /// <inheritdoc />
    public OrderResponse Create(OrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            TotalValue = request.TotalValue,
            OrderDate = request.OrderDate,
        };

        orderRepository.Add(order);
        return OrderResponse.FromDomain(order);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        return orderRepository.Delete(id);
    }
}
