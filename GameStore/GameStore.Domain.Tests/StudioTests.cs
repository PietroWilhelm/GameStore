using GameStore.Domain.Entities;

namespace GameStore.Domain.Tests;

public class StudioTests
{
    [Fact]
    public void CriaStudio_ComDadosValidos_DeveCriarStudio()
    {
        // Arrange
        var fundacao = new DateTime(1994, 5, 30);

        // Act
        var studio = new Studio("CD Projekt Red", fundacao);

        // Assert
        Assert.Equal("CD Projekt Red", studio.Name);
        Assert.Equal(fundacao, studio.FoundationDate);
        Assert.Empty(studio.Games);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_ComNomeVazio_DeveLancarException(string nomeInvalido)
    {
        // Arrange
        var fundacao = new DateTime(2000, 1, 1);

        // Act
        var act = () => new Studio(nomeInvalido, fundacao);

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("O nome da desenvolvedora não pode ser vazio.", ex.Message);
    }

    [Fact]
    public void Construtor_ComDataDeFundacaoNoFuturo_DeveLancarException()
    {
        // Arrange
        var dataFutura = DateTime.Now.AddYears(1);

        // Act
        var act = () => new Studio("Estúdio do Futuro", dataFutura);

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("A data de fundação não pode ser no futuro.", ex.Message);
    }
}
