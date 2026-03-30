using GodGames.Domain.Entities;
using GodGames.Domain.Enums;

namespace GodGames.Application.Interfaces;

public interface IWorldEventRepository
{
    Task<List<WorldEvent>> GetByBiomeAsync(Biome biome, CancellationToken ct = default);
    Task<List<WorldEvent>> GetAllAsync(CancellationToken ct = default);
}
