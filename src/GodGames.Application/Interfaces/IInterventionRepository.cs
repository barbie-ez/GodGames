using GodGames.Domain.Entities;

namespace GodGames.Application.Interfaces;

public interface IInterventionRepository
{
    Task<Intervention?> GetPendingByGodIdAsync(Guid godId, CancellationToken ct = default);
    Task AddAsync(Intervention intervention, CancellationToken ct = default);
    Task MarkAppliedAsync(Guid interventionId, CancellationToken ct = default);
}
