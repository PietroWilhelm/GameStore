using GameStore.Application.DTOs;
using GameStore.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os clientes cadastrados na loja.
/// </summary>
// Recurso não versionado
[ApiVersionNeutral]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    /// <summary>
    /// Lista todos os clientes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(customerService.GetAll());

    /// <summary>
    /// Busca um cliente pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var customer = customerService.GetById(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CustomerRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var created = customerService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Remove um cliente pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => customerService.Delete(id) ? NoContent() : NotFound();
}
