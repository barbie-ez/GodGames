using GodGames.Application.Events;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Application.Services;
using GodGames.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GodGames.Application.Jobs;

public class WorldTickJob(
    IChampionRepository champions,
    IInterventionRepository interventions,
    IWorldEventRepository worldEvents,
    IWorldRegionRepository worldRegions,
    IGodRepository gods,
    INarrativeRepository narratives,
    IGameEngineService engine,
    InterventionParser parser,
    IMediator mediator,
    ITickNotifier tickNotifier,
    ILogger<WorldTickJob> logger)
{
    private static int _tickNumber;
    private static bool _tickNumberInitialised;
    private static readonly SemaphoreSlim _initLock = new(1, 1);
    private static readonly Random Rng = new();

    public async Task ExecuteAsync()
    {
        // Seed tick number from DB on first run so it survives process restarts.
        if (!_tickNumberInitialised)
        {
            await _initLock.WaitAsync();
            try
            {
                if (!_tickNumberInitialised)
                {
                    var maxTick = await narratives.GetMaxTickNumberAsync();
                    Interlocked.Exchange(ref _tickNumber, maxTick);
                    _tickNumberInitialised = true;
                    logger.LogInformation("Tick counter initialised from DB: last tick was {MaxTick}", maxTick);
                }
            }
            finally { _initLock.Release(); }
        }

        var tickNumber = Interlocked.Increment(ref _tickNumber);
        logger.LogInformation("World tick {TickNumber} started", tickNumber);

        var allChampions = await champions.GetAllActiveAsync();
        logger.LogInformation("Processing {Count} champions", allChampions.Count);

        foreach (var champion in allChampions)
        {
            try
            {
                // 1. Roll luck for this champion (seeded per champion per tick)
                var luckOutcome = engine.LuckRoll(champion.Id, tickNumber);

                // 2. Fetch pending intervention, or carry over active power-up
                StatEffect? statEffect = null;
                var intervention = await interventions.GetPendingByGodIdAsync(champion.GodId);
                if (intervention is not null)
                {
                    var parsed = parser.ParseWithPersonality(intervention.RawCommand, champion.PersonalityTrait);
                    statEffect = parsed;

                    // Handle move-to-region command
                    if (parsed.MoveToRegionId is not null)
                    {
                        await TryMoveChampionAsync(champion, parsed.MoveToRegionId);
                        statEffect = null; // movement consumes the intervention slot, no stat effect
                    }
                    else if (parsed.DurationTicks > 0)
                    {
                        champion.PowerUpSlot = intervention.RawCommand;
                        champion.PowerUpTicksRemaining = parsed.DurationTicks - 1;
                    }
                    else
                    {
                        champion.PowerUpSlot = null;
                        champion.PowerUpTicksRemaining = 0;
                    }
                    logger.LogDebug("Champion {ChampionId} new intervention: {Command} (duration {D})",
                        champion.Id, intervention.RawCommand, parsed.DurationTicks);
                    await interventions.MarkAppliedAsync(intervention.Id);
                }
                else if (champion.PowerUpSlot is not null && champion.PowerUpTicksRemaining > 0)
                {
                    statEffect = parser.ParseWithPersonality(champion.PowerUpSlot, champion.PersonalityTrait) with { HP = 0 };
                    champion.PowerUpTicksRemaining--;
                    if (champion.PowerUpTicksRemaining == 0)
                        champion.PowerUpSlot = null;
                    logger.LogDebug("Champion {ChampionId} power-up carry-over: {Slot} ({R} ticks left after this)",
                        champion.Id, champion.PowerUpSlot ?? "(expired)", champion.PowerUpTicksRemaining);
                }

                // 3. Select a random world event matching champion's biome
                // Personality influences event pool selection
                var biomeEvents = await worldEvents.GetByBiomeAsync(champion.Biome);
                if (biomeEvents.Count == 0)
                {
                    logger.LogWarning("No world events found for biome {Biome}", champion.Biome);
                    continue;
                }
                var worldEvent = SelectEventForPersonality(biomeEvents, champion.PersonalityTrait);

                // 4. Resolve the event (mutates champion in-place, includes luck + debuffs)
                var outcome = engine.ResolveEvent(champion, worldEvent, statEffect, luckOutcome);

                // 5. Evaluate and update patron title
                var newTitle = PatronTitleService.EvaluateTitle(champion);
                await gods.UpdatePatronTitleAsync(champion.GodId, newTitle);

                // 6. Persist champion state
                await champions.UpdateAsync(champion);

                // 7. Publish TickResolved — triggers NarrativeService
                await mediator.Publish(new TickResolved(champion, worldEvent, outcome, tickNumber));

                // 8. Push ChampionUpdated to connected dashboard via Redis → SignalR
                var dto = CreateChampionDto(champion);
                await tickNotifier.NotifyChampionUpdatedAsync(champion.GodId, dto);

                logger.LogInformation(
                    "Tick {TickNumber}: champion {Name} [{Trait}] completed {EventName} — {XP} XP gained, HP {HP}/{MaxHP}, luck={Luck}{LevelUp}",
                    tickNumber, champion.Name, champion.PersonalityTrait, worldEvent.Name, outcome.XpGained,
                    champion.HP, champion.MaxHP, luckOutcome, outcome.LeveledUp ? " [LEVEL UP]" : "");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Tick {TickNumber} failed for champion {ChampionId}", tickNumber, champion.Id);
            }
        }

        logger.LogInformation("World tick {TickNumber} completed", tickNumber);
    }

    private async Task TryMoveChampionAsync(Domain.Entities.Champion champion, string targetRegionId)
    {
        var region = await worldRegions.GetByIdAsync(targetRegionId);
        if (region is null)
        {
            logger.LogWarning("Move command: region '{RegionId}' not found for champion {ChampionId}",
                targetRegionId, champion.Id);
            return;
        }

        if (champion.Level < region.MinLevelRequired)
        {
            logger.LogInformation("Champion {ChampionId} level {Level} too low for region {Region} (min {Min})",
                champion.Id, champion.Level, targetRegionId, region.MinLevelRequired);
            return;
        }

        champion.CurrentRegionId = targetRegionId;

        // Add to explored regions if not already present
        var explored = System.Text.Json.JsonSerializer.Deserialize<List<string>>(champion.ExploredRegionIds) ?? [];
        if (!explored.Contains(targetRegionId))
        {
            explored.Add(targetRegionId);
            champion.ExploredRegionIds = System.Text.Json.JsonSerializer.Serialize(explored);
        }

        logger.LogInformation("Champion {ChampionId} moved to region {Region}", champion.Id, targetRegionId);
    }

    private static Domain.Entities.WorldEvent SelectEventForPersonality(
        List<Domain.Entities.WorldEvent> events,
        PersonalityTrait trait)
    {
        // Brave/Reckless: bias toward events with higher stat requirements (harder = more combat-like)
        // Cautious: bias toward easier events (lower or no stat requirements)
        // Cunning: slight bias toward variety (just random for now — loot is a narrative concept)
        // For MVP: use weighted random — repeat "preferred" events to increase their selection chance
        if (events.Count == 1) return events[0];

        var weighted = new List<Domain.Entities.WorldEvent>(events);

        if (trait is PersonalityTrait.Brave or PersonalityTrait.Reckless)
        {
            // Add combat-heavy events (those with stat requirements) twice more
            var combatEvents = events.Where(e => e.StatRequirementsJson != "{}").ToList();
            weighted.AddRange(combatEvents);
            if (trait == PersonalityTrait.Reckless)
                weighted.AddRange(combatEvents); // triple weight for Reckless
        }
        else if (trait == PersonalityTrait.Cautious)
        {
            // Add safe/exploration events (no stat requirements) twice more
            var safeEvents = events.Where(e => e.StatRequirementsJson == "{}").ToList();
            weighted.AddRange(safeEvents);
        }

        return weighted[Rng.Next(weighted.Count)];
    }

    private static ChampionDto CreateChampionDto(Domain.Entities.Champion c) => new(
        c.Id, c.GodId, c.Name, c.Class, c.PersonalityTrait,
        c.Stats.STR, c.Stats.DEX, c.Stats.INT, c.Stats.WIS, c.Stats.VIT,
        c.HP, c.MaxHP, c.Level, c.XP, c.PowerUpSlot, c.PowerUpTicksRemaining, c.Biome,
        c.ActiveDebuff, c.ActiveDebuffTicksRemaining, c.CurrentRegionId, c.ExploredRegionIds,
        c.CreatedAt, c.LastTickAt);
}
