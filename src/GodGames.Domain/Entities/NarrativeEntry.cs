namespace GodGames.Domain.Entities;

public class NarrativeEntry
{
    public Guid Id { get; set; }
    public Guid ChampionId { get; set; }
    public int TickNumber { get; set; }
    public string StoryText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public Champion Champion { get; set; } = null!;
}
