using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using GodGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class WorldRegionRepository(GodGamesDbContext db) : IWorldRegionRepository
{
    public Task<List<WorldRegion>> GetAllAsync(CancellationToken ct = default)
        => db.WorldRegions.AsNoTracking().ToListAsync(ct);

    public Task<WorldRegion?> GetByIdAsync(string regionId, CancellationToken ct = default)
        => db.WorldRegions.AsNoTracking().FirstOrDefaultAsync(r => r.Id == regionId, ct);
}
