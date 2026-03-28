using GodGames.Domain.Enums;

namespace GodGames.Domain.Entities;

public class WorldRegion
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Biome Biome { get; set; }
    public int DifficultyRating { get; set; }  // 1-10
    public int MinLevelRequired { get; set; }
    public int MapX { get; set; }
    public int MapY { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ActiveEventTypes { get; set; } = string.Empty;
}
