namespace GameStore.Domain.Tests;

public class CustomerTests
{
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
}
