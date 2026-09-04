using Moq;
using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Implementations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Tests.Services.Implementations;

public class StudioServiceTests
{
    private readonly Mock<IStudioRepository> _studios = new();
    private readonly StudioService _studioService;

    public StudioServiceTests()
    {
        _studioService = new StudioService(_studios.Object);
    }

    [Fact]
    public void Create_ComDadosValidos_DevePersistirUmaVezERetornarDadosMapeados()
    {
        // Arrange
        var request = new StudioRequest("Naughty Dog", new DateTime(1984, 9, 27));

        // Act
        var result = _studioService.Create(request);

        // Assert
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.FoundationDate, result.FoundationDate);
        _studios.Verify(r => r.Add(It.IsAny<Studio>()), Times.Once);
    }
}
