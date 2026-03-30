using GodGames.Domain.Entities;
using GodGames.Domain.Enums;

namespace GodGames.Infrastructure.Persistence.Seed;

public static class WorldEventSeed
{
    public static IEnumerable<WorldEvent> GetEvents() =>
    [
        // Safe biome — 7 events, minimal stat requirements
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000001"),
            Name = "Village Market",
            Description = "A bustling market where merchants hawk their wares and travelers share tales.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000002"),
            Name = "Forest Path",
            Description = "A well-worn trail through a peaceful woodland.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000003"),
            Name = "Roadside Inn",
            Description = "A warm inn offering rest and rumours.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000004"),
            Name = "Pilgrim's Road",
            Description = "A sacred road walked by devout pilgrims seeking distant shrines.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000005"),
            Name = "Merchant Caravan",
            Description = "A trading caravan offering exotic goods from distant lands.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000006"),
            Name = "Ancient Ruins",
            Description = "Crumbling ruins of a forgotten civilisation, now largely explored and safe.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{\"WIS\": 8}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("10000000-0000-0000-0000-000000000007"),
            Name = "Healing Spring",
            Description = "A mystical spring with restorative waters blessed by forgotten gods.",
            Biome = Biome.Safe,
            StatRequirementsJson = "{}",
            OutcomeModifiersJson = "{}"
        },

        // Normal biome — 7 events, moderate stat requirements
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000001"),
            Name = "Goblin Ambush",
            Description = "A gang of goblins leaps from the treeline to harass travellers.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"STR\": 12}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000002"),
            Name = "Bandit Camp",
            Description = "A fortified camp of outlaws blocking the road ahead.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"DEX\": 12}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000003"),
            Name = "Dark Forest",
            Description = "An ancient wood where malevolent spirits lead travellers astray.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"WIS\": 12}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000004"),
            Name = "Haunted Crypt",
            Description = "An old burial chamber whose undead residents resent intruders.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"INT\": 12}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000005"),
            Name = "Mountain Pass",
            Description = "A treacherous mountain crossing battered by harsh winds.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"VIT\": 12}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000006"),
            Name = "River Crossing",
            Description = "A swollen river where the current threatens to sweep travellers away.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"STR\": 10}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("20000000-0000-0000-0000-000000000007"),
            Name = "Troll Bridge",
            Description = "A hulking troll demands payment — or combat — to cross its bridge.",
            Biome = Biome.Normal,
            StatRequirementsJson = "{\"STR\": 14}",
            OutcomeModifiersJson = "{}"
        },

        // Dangerous biome — 6 events, high stat requirements
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000001"),
            Name = "Dragon's Lair",
            Description = "The lair of an ancient dragon hoarding centuries of plunder.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"STR\": 18}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000002"),
            Name = "Necromancer's Tower",
            Description = "A dark spire where a lich commands an army of the undead.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"INT\": 18}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000003"),
            Name = "Assassin's Guild",
            Description = "The shadow compound of a deadly guild who silence loose ends.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"DEX\": 18}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000004"),
            Name = "Elder Demon",
            Description = "An ancient demon of immense power seeking a soul to devour.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"VIT\": 18}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000005"),
            Name = "Titan's Keep",
            Description = "A fortress built by giants and still guarded by their descendants.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"STR\": 15, \"VIT\": 15, \"WIS\": 15}",
            OutcomeModifiersJson = "{}"
        },
        new WorldEvent
        {
            Id = new Guid("30000000-0000-0000-0000-000000000006"),
            Name = "Void Rift",
            Description = "A tear in reality that draws the unwary into the consuming void.",
            Biome = Biome.Dangerous,
            StatRequirementsJson = "{\"WIS\": 18}",
            OutcomeModifiersJson = "{}"
        },
    ];
}
