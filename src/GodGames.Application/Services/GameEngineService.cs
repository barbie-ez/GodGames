using System.Text.Json;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Entities;
using GodGames.Domain.ValueObjects;

namespace GodGames.Application.Services;

public class GameEngineService : IGameEngineService
{
    private static readonly Random Rng = new();

    public TickOutcome ResolveEvent(Champion champion, WorldEvent worldEvent, StatEffect? interventionEffect)
    {
        // Apply intervention stat boosts temporarily for this tick
        var effectiveStats = ApplyEffect(champion.Stats, interventionEffect);

        // Parse stat requirements from world event
        var requirements = ParseStatRequirements(worldEvent.StatRequirementsJson);

        // Determine success based on effective stats vs requirements
        bool meetsRequirements = MeetsRequirements(effectiveStats, requirements);
        int successRoll = Rng.Next(1, 101);

        // Success chance: 80% if meets requirements, 40% otherwise
        bool success = meetsRequirements ? successRoll <= 80 : successRoll <= 40;

        // Calculate outcomes
        int xpGained = success ? Rng.Next(20, 51) : Rng.Next(5, 21);
        int hpDelta = success ? Rng.Next(0, 16) : -Rng.Next(5, 21);

        // Apply HP intervention bonus
        if (interventionEffect?.HP > 0)
            hpDelta += interventionEffect.HP;

        // Clamp HP
        int newHp = Math.Clamp(champion.HP + hpDelta, 1, champion.MaxHP);
        hpDelta = newHp - champion.HP;

        // Apply changes to champion
        champion.HP = newHp;
        champion.XP += xpGained;
        champion.LastTickAt = DateTimeOffset.UtcNow;

        // Level up check: XP threshold = Level * 100
        bool leveledUp = false;
        while (champion.XP >= champion.Level * 100)
        {
            champion.XP -= champion.Level * 100;
            champion.Level++;
            champion.MaxHP += 10;
            champion.HP = Math.Min(champion.HP + 10, champion.MaxHP);

            // Increase base stats on level up
            champion.Stats = champion.Stats with
            {
                STR = champion.Stats.STR + 1,
                DEX = champion.Stats.DEX + 1,
                INT = champion.Stats.INT + 1,
                WIS = champion.Stats.WIS + 1,
                VIT = champion.Stats.VIT + 1
            };
            leveledUp = true;
        }

        string description = success
            ? $"succeeded in the {worldEvent.Name} encounter, gaining {xpGained} XP"
            : $"struggled through {worldEvent.Name}, taking {-hpDelta} damage but earning {xpGained} XP";

        return new TickOutcome(champion.Id, xpGained, hpDelta, worldEvent.Name, description, leveledUp);
    }

    private static Stats ApplyEffect(Stats stats, StatEffect? effect)
    {
        if (effect is null) return stats;
        return stats with
        {
            STR = stats.STR + effect.STR,
            DEX = stats.DEX + effect.DEX,
            INT = stats.INT + effect.INT,
            WIS = stats.WIS + effect.WIS,
            VIT = stats.VIT + effect.VIT
        };
    }

    private static Dictionary<string, int> ParseStatRequirements(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "{}")
            return [];

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static bool MeetsRequirements(Stats stats, Dictionary<string, int> requirements)
    {
        foreach (var (stat, threshold) in requirements)
        {
            int value = stat.ToUpperInvariant() switch
            {
                "STR" => stats.STR,
                "DEX" => stats.DEX,
                "INT" => stats.INT,
                "WIS" => stats.WIS,
                "VIT" => stats.VIT,
                _ => 0
            };
            if (value < threshold) return false;
        }
        return true;
    }
}
