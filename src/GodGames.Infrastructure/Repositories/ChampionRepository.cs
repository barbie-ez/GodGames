using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using GodGames.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class ChampionRepository(GodGamesDbContext db) : IChampionRepository
{
    public Task<Champion?> GetByGodIdAsync(Guid godId, CancellationToken ct = default)
        => db.Champions.FirstOrDefaultAsync(c => c.GodId == godId, ct);

    public Task<Champion?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Champions.FindAsync([id], ct).AsTask();

    public Task<List<Champion>> GetAllActiveAsync(CancellationToken ct = default)
        => db.Champions.ToListAsync(ct);

    public async Task AddAsync(Champion champion, CancellationToken ct = default)
    {
        db.Champions.Add(champion);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Champion champion, CancellationToken ct = default)
    {
        db.Champions.Update(champion);
        await db.SaveChangesAsync(ct);
    }
}
