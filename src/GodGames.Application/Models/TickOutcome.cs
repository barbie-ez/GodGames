namespace GodGames.Application.Models;

public record TickOutcome(
    Guid ChampionId,
    int XpGained,
    int HpDelta,
    string EventName,
    string OutcomeDescription,
    bool LeveledUp = false);
