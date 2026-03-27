using GodGames.Domain.Enums;

namespace GodGames.Application.Models;

public record ChampionDto(
    Guid Id,
    Guid GodId,
    string Name,
    ChampionClass Class,
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
    Biome Biome,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastTickAt);
