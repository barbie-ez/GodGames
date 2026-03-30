using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;
using GodGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class WorldEventRepository(GodGamesDbContext db) : IWorldEventRepository
{
    public Task<List<WorldEvent>> GetByBiomeAsync(Biome biome, CancellationToken ct = default)
        => db.WorldEvents.Where(e => e.Biome == biome).ToListAsync(ct);

    public Task<List<WorldEvent>> GetAllAsync(CancellationToken ct = default)
        => db.WorldEvents.ToListAsync(ct);
}
