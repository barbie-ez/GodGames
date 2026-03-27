namespace GodGames.Application.Models;

public record InterventionDto(
    Guid Id,
    Guid GodId,
    Guid ChampionId,
    string RawCommand,
    bool IsApplied,
    DateTimeOffset CreatedAt);
