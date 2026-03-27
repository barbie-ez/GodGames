using GodGames.Application.Events;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GodGames.Worker.Jobs;

public class WorldTickJob(
    IChampionRepository champions,
    IInterventionRepository interventions,
    IWorldEventRepository worldEvents,
    IGameEngineService engine,
    IInterventionParser parser,
    IMediator mediator,
    ITickNotifier tickNotifier,
    ILogger<WorldTickJob> logger)
{
    private static int _tickNumber;
    private static readonly Random Rng = new();

    public async Task ExecuteAsync()
    {
        var tickNumber = Interlocked.Increment(ref _tickNumber);
        logger.LogInformation("World tick {TickNumber} started", tickNumber);

        var allChampions = await champions.GetAllActiveAsync();
        logger.LogInformation("Processing {Count} champions", allChampions.Count);

        foreach (var champion in allChampions)
        {
            try
            {
                // 1. Fetch pending intervention and parse effect
                StatEffect? statEffect = null;
                var intervention = await interventions.GetPendingByGodIdAsync(champion.GodId);
                if (intervention is not null)
                {
                    statEffect = parser.Parse(intervention.RawCommand);
                    logger.LogDebug("Champion {ChampionId} has intervention: {Command}",
                        champion.Id, intervention.RawCommand);
                }

                // 2. Select a random world event matching champion's biome
                var biomeEvents = await worldEvents.GetByBiomeAsync(champion.Biome);
                if (biomeEvents.Count == 0)
                {
                    logger.LogWarning("No world events found for biome {Biome}", champion.Biome);
                    continue;
                }
                var worldEvent = biomeEvents[Rng.Next(biomeEvents.Count)];

                // 3. Resolve the event (mutates champion in-place)
                var outcome = engine.ResolveEvent(champion, worldEvent, statEffect);

                // 4. Persist champion state
                await champions.UpdateAsync(champion);

                // 5. Mark intervention applied
                if (intervention is not null)
                    await interventions.MarkAppliedAsync(intervention.Id);

                // 6. Publish TickResolved — triggers NarrativeService
                await mediator.Publish(new TickResolved(champion, worldEvent, outcome, tickNumber));

                // 7. Push ChampionUpdated to connected dashboard via Redis → SignalR
                var dto = CreateChampionDto(champion);
                await tickNotifier.NotifyChampionUpdatedAsync(champion.GodId, dto);

                logger.LogInformation(
                    "Tick {TickNumber}: champion {Name} completed {EventName} — {XP} XP gained, HP {HP}/{MaxHP}{LevelUp}",
                    tickNumber, champion.Name, worldEvent.Name, outcome.XpGained,
                    champion.HP, champion.MaxHP, outcome.LeveledUp ? " [LEVEL UP]" : "");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Tick {TickNumber} failed for champion {ChampionId}", tickNumber, champion.Id);
            }
        }

        logger.LogInformation("World tick {TickNumber} completed", tickNumber);
    }

    private static ChampionDto CreateChampionDto(GodGames.Domain.Entities.Champion c) => new(
        c.Id, c.GodId, c.Name, c.Class,
        c.Stats.STR, c.Stats.DEX, c.Stats.INT, c.Stats.WIS, c.Stats.VIT,
        c.HP, c.MaxHP, c.Level, c.XP, c.PowerUpSlot, c.Biome,
        c.CreatedAt, c.LastTickAt);
}
