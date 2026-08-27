using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Controllers;

/// <summary>
/// Gerencia os clientes cadastrados na loja.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CustomerController(ICustomerRepository customerRepository) : ControllerBase
{
    /// <summary>
    /// Lista todos os clientes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Customer>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
        => Ok(customerRepository.GetAll());

    /// <summary>
    /// Busca um cliente pelo identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        var customer = customerRepository.GetById(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CustomerRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (customerRepository.GetByEmail(request.Email) is not null)
            return BadRequest("Já existe um cliente com este e-mail.");

        if (customerRepository.GetByCpf(request.Cpf) is not null)
            return BadRequest("Já existe um cliente com este CPF.");

        try
        {
            var customer = new Customer(request.Name, request.Email, request.BirthDate, request.Password, request.Cpf);
            return Ok(customerRepository.Add(customer));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Remove um cliente pelo identificador.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
        => customerRepository.Delete(id) ? NoContent() : NotFound();
}
