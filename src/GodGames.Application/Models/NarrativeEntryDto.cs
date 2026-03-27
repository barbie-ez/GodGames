namespace GodGames.Application.Models;

public record NarrativeEntryDto(
    Guid Id,
    Guid ChampionId,
    int TickNumber,
    string StoryText,
    DateTimeOffset CreatedAt);
