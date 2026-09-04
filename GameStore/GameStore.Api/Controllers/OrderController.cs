using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os pedidos realizados pelos clientes.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Lista todos os pedidos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(orderService.GetAll());

    /// <summary>
    /// Lista os pedidos de um cliente específico.
    /// </summary>
    [HttpGet("customers/{customerId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public IActionResult GetByCustomer(Guid customerId)
        => Ok(orderService.GetByCustomer(customerId));

    /// <summary>
    /// Busca um pedido pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var order = orderService.GetById(id);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] OrderRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = orderService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Remove um pedido pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => orderService.Delete(id) ? NoContent() : NotFound();
}
