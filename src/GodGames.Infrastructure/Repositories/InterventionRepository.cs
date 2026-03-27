using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using GodGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class InterventionRepository(GodGamesDbContext db) : IInterventionRepository
{
    public Task<Intervention?> GetPendingByGodIdAsync(Guid godId, CancellationToken ct = default)
        => db.Interventions
            .Where(i => i.GodId == godId && !i.IsApplied)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(Intervention intervention, CancellationToken ct = default)
    {
        db.Interventions.Add(intervention);
        await db.SaveChangesAsync(ct);
    }

    public async Task MarkAppliedAsync(Guid interventionId, CancellationToken ct = default)
    {
        await db.Interventions
            .Where(i => i.Id == interventionId)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsApplied, true), ct);
    }
}
