using GodGames.Domain.Entities;
using GodGames.Domain.Enums;

namespace GodGames.Infrastructure.Persistence.Seed;

public static class WorldRegionSeed
{
    // SVG viewBox="0 0 400 600" — Safe=south (y 400-550), Normal=centre (y 200-380), Dangerous=north (y 30-190)
    public static IEnumerable<WorldRegion> GetRegions() =>
    [
        // Safe biome — south (green tones)
        new WorldRegion
        {
            Id = "whispering-fields",
            Name = "Whispering Fields",
            Biome = Biome.Safe,
            DifficultyRating = 1,
            MinLevelRequired = 1,
            MapX = 80,
            MapY = 480,
            Description = "Gentle grasslands where young champions first find their footing.",
            ActiveEventTypes = "Exploration, Rest, Trade"
        },
        new WorldRegion
        {
            Id = "greenwatch-village",
            Name = "Greenwatch Village",
            Biome = Biome.Safe,
            DifficultyRating = 2,
            MinLevelRequired = 1,
            MapX = 200,
            MapY = 510,
            Description = "A welcoming village with markets, healers, and rumours of glory.",
            ActiveEventTypes = "Trade, Rest, Healing"
        },
        new WorldRegion
        {
            Id = "sunken-grove",
            Name = "Sunken Grove",
            Biome = Biome.Safe,
            DifficultyRating = 3,
            MinLevelRequired = 2,
            MapX = 330,
            MapY = 460,
            Description = "Ancient ruins half-buried in woodland, with minor creatures lurking.",
            ActiveEventTypes = "Exploration, Combat, Loot"
        },

        // Normal biome — centre (amber tones)
        new WorldRegion
        {
            Id = "ironveil-pass",
            Name = "Ironveil Pass",
            Biome = Biome.Normal,
            DifficultyRating = 4,
            MinLevelRequired = 5,
            MapX = 70,
            MapY = 300,
            Description = "A treacherous mountain pass frequented by bandits and desperate travellers.",
            ActiveEventTypes = "Combat, Ambush, Trade"
        },
        new WorldRegion
        {
            Id = "ashwood-crossing",
            Name = "Ashwood Crossing",
            Biome = Biome.Normal,
            DifficultyRating = 5,
            MinLevelRequired = 5,
            MapX = 200,
            MapY = 280,
            Description = "A river crossing through a charred forest, haunted by restless spirits.",
            ActiveEventTypes = "Combat, Exploration, Curse"
        },
        new WorldRegion
        {
            Id = "the-bleached-crypt",
            Name = "The Bleached Crypt",
            Biome = Biome.Normal,
            DifficultyRating = 6,
            MinLevelRequired = 7,
            MapX = 330,
            MapY = 320,
            Description = "An ancient necropolis filled with undead guardians and forgotten treasure.",
            ActiveEventTypes = "Combat, Loot, Undead"
        },

        // Dangerous biome — north (red/coral tones)
        new WorldRegion
        {
            Id = "dragonspine-ridge",
            Name = "Dragonspine Ridge",
            Biome = Biome.Dangerous,
            DifficultyRating = 8,
            MinLevelRequired = 10,
            MapX = 80,
            MapY = 120,
            Description = "Volcanic ridgelines where dragons nest and the air tastes of sulphur.",
            ActiveEventTypes = "Combat, Dragon, Fire"
        },
        new WorldRegion
        {
            Id = "void-threshold",
            Name = "Void Threshold",
            Biome = Biome.Dangerous,
            DifficultyRating = 9,
            MinLevelRequired = 10,
            MapX = 200,
            MapY = 80,
            Description = "A rift in reality where demon lords breach the mortal plane.",
            ActiveEventTypes = "Combat, Demon, Void"
        },
        new WorldRegion
        {
            Id = "the-obsidian-keep",
            Name = "The Obsidian Keep",
            Biome = Biome.Dangerous,
            DifficultyRating = 10,
            MinLevelRequired = 12,
            MapX = 330,
            MapY = 110,
            Description = "Fortress of the Necromancer-King — only the mightiest dare enter.",
            ActiveEventTypes = "Combat, Boss, Necromancy"
        },
    ];
}
