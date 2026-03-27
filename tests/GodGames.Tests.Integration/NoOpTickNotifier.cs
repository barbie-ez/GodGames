using GodGames.Application.Interfaces;
using GodGames.Application.Models;

namespace GodGames.Tests.Integration;

/// <summary>No-op ITickNotifier — prevents Redis from being contacted during tests.</summary>
public class NoOpTickNotifier : ITickNotifier
{
    public Task NotifyChampionUpdatedAsync(Guid godId, ChampionDto champion, CancellationToken ct = default)
        => Task.CompletedTask;
}
