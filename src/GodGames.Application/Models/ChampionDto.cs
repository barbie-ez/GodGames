using GodGames.Domain.Enums;

namespace GodGames.Application.Models;

public record ChampionDto(
    Guid Id,
    Guid GodId,
    string Name,
    ChampionClass Class,
    PersonalityTrait PersonalityTrait,
    int STR,
    int DEX,
    int INT,
    int WIS,
    int VIT,
    int HP,
    int MaxHP,
    int Level,
    int XP,
    string? PowerUpSlot,
    int PowerUpTicksRemaining,
    Biome Biome,
    DebuffType ActiveDebuff,
    int ActiveDebuffTicksRemaining,
    string CurrentRegionId,
    string ExploredRegionIds,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastTickAt);
