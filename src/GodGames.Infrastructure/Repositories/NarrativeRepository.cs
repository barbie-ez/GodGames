using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using GodGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class NarrativeRepository(GodGamesDbContext db) : INarrativeRepository
{
    public async Task AddAsync(NarrativeEntry entry, CancellationToken ct = default)
    {
        db.NarrativeEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public Task<List<NarrativeEntry>> GetLastNByChampionIdAsync(Guid championId, int count, CancellationToken ct = default)
        => db.NarrativeEntries
            .Where(n => n.ChampionId == championId)
            .OrderByDescending(n => n.TickNumber)
            .Take(count)
            .ToListAsync(ct);
}
