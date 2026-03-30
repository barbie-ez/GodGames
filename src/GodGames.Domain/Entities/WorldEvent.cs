using GodGames.Domain.Enums;

namespace GodGames.Domain.Entities;

public class WorldEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Biome Biome { get; set; }
    public string StatRequirementsJson { get; set; } = "{}";
    public string OutcomeModifiersJson { get; set; } = "{}";
}
