using GodGames.Application.Events;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GodGames.Application.Jobs;

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
                // 1. Fetch pending intervention, or carry over active power-up
                StatEffect? statEffect = null;
                var intervention = await interventions.GetPendingByGodIdAsync(champion.GodId);
                if (intervention is not null)
                {
                    var parsed = parser.Parse(intervention.RawCommand);
                    statEffect = parsed;
                    if (parsed.DurationTicks > 0)
                    {
                        // Store as active power-up; already applying this tick so -1
                        champion.PowerUpSlot = intervention.RawCommand;
                        champion.PowerUpTicksRemaining = parsed.DurationTicks - 1;
                    }
                    else
                    {
                        // One-shot (e.g. heal, divine shield) — clear any existing power-up
                        champion.PowerUpSlot = null;
                        champion.PowerUpTicksRemaining = 0;
                    }
                    logger.LogDebug("Champion {ChampionId} new intervention: {Command} (duration {D})",
                        champion.Id, intervention.RawCommand, parsed.DurationTicks);
                    await interventions.MarkAppliedAsync(intervention.Id);
                }
                else if (champion.PowerUpSlot is not null && champion.PowerUpTicksRemaining > 0)
                {
                    // Carry over active power-up (stat boosts only — HP bonuses are one-shot)
                    statEffect = parser.Parse(champion.PowerUpSlot) with { HP = 0 };
                    champion.PowerUpTicksRemaining--;
                    if (champion.PowerUpTicksRemaining == 0)
                        champion.PowerUpSlot = null;
                    logger.LogDebug("Champion {ChampionId} power-up carry-over: {Slot} ({R} ticks left after this)",
                        champion.Id, champion.PowerUpSlot ?? "(expired)", champion.PowerUpTicksRemaining);
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

                // 5. Publish TickResolved — triggers NarrativeService
                await mediator.Publish(new TickResolved(champion, worldEvent, outcome, tickNumber));

                // 6. Push ChampionUpdated to connected dashboard via Redis → SignalR
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

    private static ChampionDto CreateChampionDto(Domain.Entities.Champion c) => new(
        c.Id, c.GodId, c.Name, c.Class,
        c.Stats.STR, c.Stats.DEX, c.Stats.INT, c.Stats.WIS, c.Stats.VIT,
        c.HP, c.MaxHP, c.Level, c.XP, c.PowerUpSlot, c.PowerUpTicksRemaining, c.Biome,
        c.CreatedAt, c.LastTickAt);
}
