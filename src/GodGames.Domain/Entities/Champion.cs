using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;

namespace GodGames.Domain.Entities;

public class Champion
{
    public Guid Id { get; set; }
    public Guid GodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ChampionClass Class { get; set; }
    public Stats Stats { get; set; } = new(10, 10, 10, 10, 10);
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Level { get; set; } = 1;
    public int XP { get; set; }
    public string? PowerUpSlot { get; set; }
    public Biome Biome { get; set; } = Biome.Safe;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastTickAt { get; set; }

    public God God { get; set; } = null!;
    public ICollection<Intervention> Interventions { get; set; } = [];
    public ICollection<NarrativeEntry> NarrativeEntries { get; set; } = [];
}
