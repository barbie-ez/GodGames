using GodGames.Domain.Enums;

namespace GodGames.Application.Interfaces;

public interface IGodRepository
{
    Task UpdatePatronTitleAsync(Guid godId, PatronTitle title, CancellationToken ct = default);
    Task<PatronTitle> GetPatronTitleAsync(Guid godId, CancellationToken ct = default);
}
