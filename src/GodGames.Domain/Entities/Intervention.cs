namespace GodGames.Domain.Entities;

public class Intervention
{
    public Guid Id { get; set; }
    public Guid GodId { get; set; }
    public Guid ChampionId { get; set; }
    public string RawCommand { get; set; } = string.Empty;
    public string ParsedEffectJson { get; set; } = "{}";
    public bool IsApplied { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public God God { get; set; } = null!;
    public Champion Champion { get; set; } = null!;
}
