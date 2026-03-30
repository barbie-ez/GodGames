using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;

namespace GodGames.Domain.Entities;

public class Champion
{
    public Guid Id { get; set; }
    public Guid GodId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ChampionClass Class { get; set; }
    public PersonalityTrait PersonalityTrait { get; set; }
    public Stats Stats { get; set; } = new(10, 10, 10, 10, 10);
    public int HP { get; set; }
    public int MaxHP { get; set; }
    public int Level { get; set; } = 1;
    public int XP { get; set; }
    public string? PowerUpSlot { get; set; }
    public int PowerUpTicksRemaining { get; set; }
    public Biome Biome { get; set; } = Biome.Safe;

    // Active debuff from luck system
    public DebuffType ActiveDebuff { get; set; } = DebuffType.None;
    public int ActiveDebuffTicksRemaining { get; set; }

    // World map
    public string CurrentRegionId { get; set; } = "whispering-fields";
    public string ExploredRegionIds { get; set; } = "[\"whispering-fields\"]"; // JSON array

    // Patron title tracking
    public int CombatWins { get; set; }
    public int TicksSurvivedStreak { get; set; }
    public int ConsecutiveCursedTicks { get; set; }
    public int DivineFavourCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastTickAt { get; set; }

    public ICollection<Intervention> Interventions { get; set; } = [];
    public ICollection<NarrativeEntry> NarrativeEntries { get; set; } = [];
}
