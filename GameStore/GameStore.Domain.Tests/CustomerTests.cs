namespace GameStore.Domain.Tests;

public class CustomerTests
{
    [Fact]
    public void CriaCustomer_ComDadosValidos_DeveCriarClienteComIdadeCalculada()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-25));

        // Act
        var customer = new Entities.Customer(
            "Cliente Válido",
            "cliente.valido@teste.com",
            dataNascimento,
            "senhaSegura123",
            "12345678900");

        // Assert
        Assert.Equal("Cliente Válido", customer.Name);
        Assert.Equal("cliente.valido@teste.com", customer.Email);
        Assert.Equal(25, customer.Age);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(12)]
    public void Construtor_ComMenosDe13Anos_DeveLancarException(int idade)
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-idade));

        // Act
        var act = () => new Entities.Customer(
            "Cliente Teste",
            "cliente@teste.com",
            dataNascimento,
            "senhaSegura123",
            "12345678900");

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("Usuário deve ter pelo menos 13 anos.", ex.Message);
    }

    [Fact]
    public void Construtor_ComSenhaCurta_DeveLancarException()
    {
        // Arrange
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));

        // Act
        var act = () => new Entities.Customer(
            "Cliente Teste",
            "cliente@teste.com",
            dataNascimento,
            "curta",
            "12345678900");

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("A senha deve ter pelo menos 8 caracteres.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba.com")]
    public void UpdateEmail_ComEmailInvalido_DeveLancarException(string emailInvalido)
    {
        // Arrange
        var customer = new Entities.Customer(
            "Cliente Teste",
            "cliente@teste.com",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
            "senhaSegura123",
            "12345678900");

        // Act
        var act = () => customer.UpdateEmail(emailInvalido);

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("E-mail inválido.", ex.Message);
    }
}
