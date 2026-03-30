using GodGames.Application.Models;

namespace GodGames.Application.Interfaces;

public interface ITickNotifier
{
    Task NotifyChampionUpdatedAsync(Guid godId, ChampionDto champion, CancellationToken ct = default);
}
