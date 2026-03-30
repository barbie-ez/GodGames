namespace GodGames.Web.Models;

// Enums mirroring domain
public enum ChampionClass { Warrior = 0, Mage = 1, Rogue = 2 }
public enum Biome { Safe = 0, Normal = 1, Dangerous = 2 }
public enum PersonalityTrait { Brave = 0, Cautious = 1, Reckless = 2, Cunning = 3 }
public enum PatronTitle { TheHopeful = 0, TheMerciless = 1, TheProtector = 2, TheCursed = 3, TheFortunate = 4 }
public enum DebuffType { None = 0, StatReduction = 1, WeakenedStrike = 2, CursedBlood = 3 }
public enum LuckOutcome { DivineFavour = 0, BlessedTick = 1, NormalTick = 2, GodLuckIsLow = 3, CursedTick = 4 }

// Auth
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password);
public record LoginResponse(string Token, DateTimeOffset ExpiresAt);

// Champion
public record ChampionDto(
    Guid Id, Guid GodId, string Name, ChampionClass Class, PersonalityTrait PersonalityTrait,
    int STR, int DEX, int INT, int WIS, int VIT,
    int HP, int MaxHP, int Level, int XP,
    string? PowerUpSlot, int PowerUpTicksRemaining, Biome Biome,
    DebuffType ActiveDebuff, int ActiveDebuffTicksRemaining, string CurrentRegionId, string ExploredRegionIds,
    DateTimeOffset CreatedAt, DateTimeOffset LastTickAt);

public record NarrativeEntryDto(Guid Id, Guid ChampionId, int TickNumber, string StoryText, DateTimeOffset CreatedAt);

public record LeaderboardEntryDto(
    int Rank, string Name, ChampionClass Class, PersonalityTrait PersonalityTrait,
    int Level, int XP, Biome Biome, PatronTitle PatronTitle);

public record WorldRegionDto(
    string Id, string Name, string Biome, int DifficultyRating,
    int MinLevelRequired, int MapX, int MapY,
    string Description, string ActiveEventTypes);

// Requests
public record CreateChampionRequest(string Name, ChampionClass Class, PersonalityTrait PersonalityTrait = PersonalityTrait.Brave);
public record SubmitInterventionRequest(Guid ChampionId, string RawCommand);
