using GodGames.Domain.Entities;

namespace GodGames.Application.Interfaces;

public interface IWorldRegionRepository
{
    Task<List<WorldRegion>> GetAllAsync(CancellationToken ct = default);
    Task<WorldRegion?> GetByIdAsync(string regionId, CancellationToken ct = default);
}
