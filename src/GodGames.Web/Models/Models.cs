namespace GodGames.Web.Models;

// Enums mirroring domain
public enum ChampionClass { Warrior = 0, Mage = 1, Rogue = 2 }
public enum Biome { Safe = 0, Normal = 1, Dangerous = 2 }

// Auth
public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password);
public record LoginResponse(string Token, DateTimeOffset ExpiresAt);

// Champion
public record ChampionDto(
    Guid Id, Guid GodId, string Name, ChampionClass Class,
    int STR, int DEX, int INT, int WIS, int VIT,
    int HP, int MaxHP, int Level, int XP,
    string? PowerUpSlot, Biome Biome,
    DateTimeOffset CreatedAt, DateTimeOffset LastTickAt);

public record NarrativeEntryDto(Guid Id, Guid ChampionId, int TickNumber, string StoryText, DateTimeOffset CreatedAt);

// Requests
public record CreateChampionRequest(string Name, ChampionClass Class);
public record SubmitInterventionRequest(Guid ChampionId, string RawCommand);
