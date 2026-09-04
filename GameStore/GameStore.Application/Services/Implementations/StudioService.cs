using GameStore.Application.DTOs;
using GameStore.Application.Repositories;
using GameStore.Application.Services.Interfaces;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.Implementations;

/// <summary>
/// Orquestra os casos de uso de Studio.
/// </summary>
public sealed class StudioService(IStudioRepository studioRepository) : IStudioService
{
    /// <inheritdoc />
    public IReadOnlyList<StudioResponse> GetAll()
    {
        return studioRepository.GetAll().Select(StudioResponse.FromDomain).ToList();
    }

    /// <inheritdoc />
    public StudioResponse? GetById(Guid id)
    {
        var studio = studioRepository.GetById(id);
        return studio is null ? null : StudioResponse.FromDomain(studio);
    }

    /// <inheritdoc />
    public StudioResponse Create(StudioRequest request)
    {
        var studio = new Studio(request.Name, request.FoundationDate);
        studioRepository.Add(studio);
        return StudioResponse.FromDomain(studio);
    }

    /// <inheritdoc />
    public bool Delete(Guid id)
    {
        return studioRepository.Delete(id);
    }
}
