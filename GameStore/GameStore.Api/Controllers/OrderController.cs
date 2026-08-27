using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os pedidos realizados pelos clientes.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class OrderController(IOrderRepository orderRepository) : ControllerBase
{
    /// <summary>
    /// Lista todos os pedidos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(orderRepository.GetAll());

    /// <summary>
    /// Lista os pedidos de um cliente específico.
    /// </summary>
    [HttpGet("customers/{customerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
    public IActionResult GetByCustomer(Guid customerId)
        => Ok(orderRepository.GetByCustomer(customerId));

    /// <summary>
    /// Busca um pedido pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var order = orderRepository.GetById(id);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] OrderRequest request)
    {
        var order = new Order
        {
            CustomerId = request.CustomerId,
            TotalValue = request.TotalValue,
            OrderDate = request.OrderDate,
        };

        return Ok(orderRepository.Add(order));
    }

    /// <summary>
    /// Remove um pedido pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => orderRepository.Delete(id) ? NoContent() : NotFound();
}
