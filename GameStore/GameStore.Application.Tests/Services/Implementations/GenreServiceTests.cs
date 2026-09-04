using Moq;
using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Implementations;
using GameStore.Domain.Entities;

namespace GameStore.Application.Tests.Services.Implementations;

public class GenreServiceTests
{
    private readonly Mock<IGenreRepository> _genres = new();
    private readonly GenreService _genreService;

    public GenreServiceTests()
    {
        _genreService = new GenreService(_genres.Object);
    }

    [Fact]
    public void Create_ComDadosValidos_DevePersistirUmaVezERetornarDadosMapeados()
    {
        // Arrange
        var request = new GenreRequest("RPG", "Jogos de interpretação de personagem com progressão de história.");

        // Act
        var result = _genreService.Create(request);

        // Assert
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(request.Description, result.Description);
        _genres.Verify(r => r.Add(It.IsAny<Genre>()), Times.Once);
    }
}
