using GodGames.Application.Events;
using GodGames.Application.Interfaces;
using GodGames.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GodGames.AI;

public class NarrativeService(
    IAnthropicClient anthropic,
    INarrativeRepository repo,
    ILogger<NarrativeService> logger) : INotificationHandler<TickResolved>
{
    public async Task Handle(TickResolved notification, CancellationToken ct)
    {
        NarrativeEntry entry;
        try
        {
            var prompt = PromptBuilder.Build(notification.Champion, notification.Event, notification.Outcome);
            var storyText = await anthropic.CompleteAsync(prompt, maxTokens: 200, ct);
            entry = BuildEntry(notification, storyText);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Narrative generation failed for champion {ChampionId}, using fallback",
                notification.Champion.Id);
            entry = BuildEntry(notification, BuildFallback(notification));
        }

        try
        {
            await repo.AddAsync(entry, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist narrative entry for champion {ChampionId}",
                notification.Champion.Id);
        }
    }

    private static NarrativeEntry BuildEntry(TickResolved n, string storyText) => new()
    {
        Id = Guid.NewGuid(),
        ChampionId = n.Champion.Id,
        TickNumber = n.TickNumber,
        StoryText = storyText,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static string BuildFallback(TickResolved n) =>
        $"{n.Champion.Name} ventured through {n.Event.Name} and {n.Outcome.OutcomeDescription}. " +
        $"The gods watched in silence as their champion pressed on.";
}
