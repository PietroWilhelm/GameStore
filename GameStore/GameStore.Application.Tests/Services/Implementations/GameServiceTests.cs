using Microsoft.Extensions.Logging;
using Moq;
using GameStore.Application.DTOs;
using GameStore.Application.Exceptions;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Implementations;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;

namespace GameStore.Application.Tests.Services.Implementations;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _games = new();
    private readonly Mock<IStudioRepository> _studios = new();
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameService = new GameService(_games.Object, _studios.Object, new Mock<ILogger<GameService>>().Object);
    }

    [Fact]
    public void Create_QuandoNomeJaExiste_DeveLancarExceptionENaoPersistir()
    {
        // Arrange
        var request = new GameRequest("Elden Ring", "RPG de ação", new DateTime(2022, 2, 25), Guid.NewGuid(), ContentTypeEnum.RPG);

        _games.Setup(r => r.ExistsByName(request.Name)).Returns(true);

        // Act
        var act = () => _gameService.Create(request);

        // Assert
        Assert.Throws<CreateException>(act);
        _games.Verify(r => r.Add(It.IsAny<Game>()), Times.Never);
    }

    [Fact]
    public void Create_QuandoStudioNaoExiste_DeveLancarExceptionENaoPersistir()
    {
        // Arrange
        var request = new GameRequest("Novo Jogo", "Descrição válida", new DateTime(2022, 2, 25), Guid.NewGuid(), ContentTypeEnum.RPG);

        _games.Setup(r => r.ExistsByName(request.Name)).Returns(false);
        _studios.Setup(r => r.ExistsById(request.StudioId)).Returns(false);

        // Act
        var act = () => _gameService.Create(request);

        // Assert
        Assert.Throws<CreateException>(act);
        _games.Verify(r => r.Add(It.IsAny<Game>()), Times.Never);
    }

    [Fact]
    public void Create_ComDadosValidos_DevePersistirUmaVez()
    {
        // Arrange
        var request = new GameRequest("Hollow Knight", "Metroidvania desenhado à mão", new DateTime(2017, 2, 24), Guid.NewGuid(), ContentTypeEnum.RPG);

        _games.Setup(r => r.ExistsByName(request.Name)).Returns(false);
        _studios.Setup(r => r.ExistsById(request.StudioId)).Returns(true);

        // Act
        var result = _gameService.Create(request);

        // Assert
        Assert.Equal(request.Name, result.Name);
        _games.Verify(r => r.Add(It.IsAny<Game>()), Times.Once);
    }
}
