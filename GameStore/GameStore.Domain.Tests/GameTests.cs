using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Domain.Tests;

public class GameTests
{
    [Fact]
    public void CriaGame_ComDadosValidos_DeveCriarGameInativo()
    {
        // Arrange
        var lancamento = new DateTime(2015, 3, 3);
        var studioId = Guid.NewGuid();

        // Act
        var game = new Game(
            "The Witcher 3",
            "RPG de mundo aberto.",
            lancamento,
            ContentTypeEnum.RPG,
            studioId);

        // Assert
        Assert.Equal("The Witcher 3", game.Name);
        Assert.Equal(studioId, game.StudioId);
        Assert.False(game.Active);
    }

    [Fact]
    public void Construtor_ComDescricaoVazia_DeveLancarException()
    {
        // Arrange
        var lancamento = new DateTime(2015, 1, 1);

        // Act
        var act = () => new Game("Jogo Sem Descrição", "", lancamento, ContentTypeEnum.action, Guid.NewGuid());

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("Description is empty", ex.Message);
    }

    [Fact]
    public void Construtor_ComAnoAnteriorA1958_DeveLancarException()
    {
        // Arrange
        var antesDoLimite = new DateTime(1957, 12, 31);

        // Act
        var act = () => new Game("Protótipo", "Experimento", antesDoLimite, ContentTypeEnum.others, Guid.NewGuid());

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("Year must be greater than 1958", ex.Message);
    }

    [Fact]
    public void SetStudio_ComGuidVazio_DeveLancarException()
    {
        // Arrange
        var lancamento = new DateTime(2020, 1, 1);

        // Act
        var act = () => new Game("Jogo", "Descrição válida", lancamento, ContentTypeEnum.action, Guid.Empty);

        // Assert
        var ex = Assert.Throws<Exception>(act);
        Assert.Equal("StudioId is invalid", ex.Message);
    }
}
