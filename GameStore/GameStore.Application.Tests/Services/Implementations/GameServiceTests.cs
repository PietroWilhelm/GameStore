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

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    public void GetPaged_ComPageOuPageSizeInvalidos_DeveLancarArgumentException(int page, int pageSize)
    {
        // Arrange / Act
        var act = () => _gameService.GetPaged(page, pageSize);

        // Assert
        Assert.Throws<ArgumentException>(act);
        _games.Verify(r => r.GetPaged(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void GetPaged_ComPageEPageSizeValidos_DeveRetornarEnvelopePaginadoComTotalPagesCorreto()
    {
        // Arrange
        var jogos = new List<Game>
        {
            new("Hollow Knight", "Metroidvania desenhado à mão", new DateTime(2017, 2, 24), ContentTypeEnum.RPG, Guid.NewGuid()),
            new("Elden Ring", "RPG de ação", new DateTime(2022, 2, 25), ContentTypeEnum.RPG, Guid.NewGuid())
        };

        _games.Setup(r => r.GetPaged(2, 2)).Returns((jogos, 5));

        // Act
        var result = _gameService.GetPaged(2, 2);

        // Assert
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(3, result.TotalPages);
        Assert.Equal(2, result.Items.Count);
        Assert.True(result.HasPrevious);
        Assert.True(result.HasNext);
        _games.Verify(r => r.GetPaged(2, 2), Times.Once);
    }
}
