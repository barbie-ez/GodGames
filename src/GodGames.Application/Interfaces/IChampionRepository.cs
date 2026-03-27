using GodGames.Domain.Entities;

namespace GodGames.Application.Interfaces;

public interface IChampionRepository
{
    Task<Champion?> GetByGodIdAsync(Guid godId, CancellationToken ct = default);
    Task<Champion?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Champion>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Champion champion, CancellationToken ct = default);
    Task UpdateAsync(Champion champion, CancellationToken ct = default);
}
