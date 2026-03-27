using GodGames.Domain.Entities;

namespace GodGames.Application.Interfaces;

public interface INarrativeRepository
{
    Task AddAsync(NarrativeEntry entry, CancellationToken ct = default);
    Task<List<NarrativeEntry>> GetLastNByChampionIdAsync(Guid championId, int count, CancellationToken ct = default);
}
