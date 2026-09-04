using Microsoft.Extensions.Logging;
using Moq;
using GameStore.Application.DTOs;
using GameStore.Application.Exceptions;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Implementations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Tests.Services.Implementations;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _customers = new();
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        _customerService = new CustomerService(_customers.Object, new Mock<ILogger<CustomerService>>().Object);
    }

    private static CustomerRequest CriarRequestValido() =>
        new("Cliente Teste", "cliente@teste.com", new DateOnly(1995, 6, 15), "senhaSegura123", "12345678900");

    [Fact]
    public void Create_QuandoEmailJaExiste_DeveLancarExceptionENaoPersistir()
    {
        // Arrange
        var request = CriarRequestValido();
        var existente = new Customer("Outro Cliente", request.Email, new DateOnly(1990, 1, 1), "outraSenha123", "98765432100");

        _customers.Setup(r => r.GetByEmail(request.Email)).Returns(existente);

        // Act
        var act = () => _customerService.Create(request);

        // Assert
        Assert.Throws<CreateException>(act);
        _customers.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public void Create_QuandoCpfJaExiste_DeveLancarExceptionENaoPersistir()
    {
        // Arrange
        var request = CriarRequestValido();
        var existente = new Customer("Outro Cliente", "outro@teste.com", new DateOnly(1990, 1, 1), "outraSenha123", request.Cpf);

        _customers.Setup(r => r.GetByEmail(request.Email)).Returns((Customer?)null);
        _customers.Setup(r => r.GetByCpf(request.Cpf)).Returns(existente);

        // Act
        var act = () => _customerService.Create(request);

        // Assert
        Assert.Throws<CreateException>(act);
        _customers.Verify(r => r.Add(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public void Create_ComDadosValidos_DevePersistirUmaVez()
    {
        // Arrange
        var request = CriarRequestValido();

        _customers.Setup(r => r.GetByEmail(request.Email)).Returns((Customer?)null);
        _customers.Setup(r => r.GetByCpf(request.Cpf)).Returns((Customer?)null);

        // Act
        var result = _customerService.Create(request);

        // Assert
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Cpf, result.Cpf);
        _customers.Verify(r => r.Add(It.IsAny<Customer>()), Times.Once);
    }
}
