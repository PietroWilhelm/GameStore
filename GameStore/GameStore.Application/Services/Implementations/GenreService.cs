using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Interfaces;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.Implementations;

/// <summary>
/// Orquestra os casos de uso de gênero.
/// </summary>
public sealed class GenreService(IGenreRepository genreRepository) : IGenreService
{
    /// <inheritdoc />
    public IReadOnlyList<GenreResponse> GetAll()
    {
        return genreRepository.GetAll().Select(GenreResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public GenreResponse? GetById(Guid id)
    {
        var genre = genreRepository.GetById(id);
        return genre is null ? null : GenreResponse.FromDomain(genre);
    }

    /// <inheritdoc />
    public GenreResponse Create(GenreRequest request)
    {
        var genre = new Genre(request.Name, request.Description);
        genreRepository.Add(genre);
        return GenreResponse.FromDomain(genre);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        return genreRepository.Delete(id);
    }
}
