using GodGames.Application.Models;
using GodGames.Domain.Entities;

namespace GodGames.AI;

public static class PromptBuilder
{
    public static string Build(Champion champion, WorldEvent worldEvent, TickOutcome outcome)
    {
        return $"""
            Champion: {champion.Name} (Level {champion.Level} {champion.Class})
            Stats: STR {champion.Stats.STR}, DEX {champion.Stats.DEX}, INT {champion.Stats.INT}, WIS {champion.Stats.WIS}, VIT {champion.Stats.VIT}
            HP: {champion.HP}/{champion.MaxHP}
            Event: {worldEvent.Name} — {worldEvent.Description}
            Outcome: The champion {outcome.OutcomeDescription}.{(outcome.LeveledUp ? $" They reached level {champion.Level}!" : "")}

            Narrate this moment.
            """;
    }
}
