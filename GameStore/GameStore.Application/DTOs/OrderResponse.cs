using GameStore.Domain.Entities;

namespace GameStore.Application.DTOs;

/// <summary>
/// DTO de resposta para pedido.
/// </summary>
public record OrderResponse(Guid Id, Guid CustomerId, decimal TotalValue, DateTime OrderDate)
{
    /// <summary>
    /// Mapeia <see cref="Order"/> para DTO.
    /// </summary>
    public static OrderResponse FromDomain(Order order) =>
        new(order.Id, order.CustomerId, order.TotalValue, order.OrderDate);
}
