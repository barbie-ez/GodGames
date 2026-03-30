using System.Text.Json;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;

namespace GodGames.Application.Services;

public class GameEngineService : IGameEngineService
{
    // LuckRoll is seeded per champion per tick so each champion has independent luck.
    public LuckOutcome LuckRoll(Guid championId, int tickNumber)
    {
        var seed = HashCode.Combine(championId, tickNumber);
        var rng = new Random(seed);
        int roll = rng.Next(1, 101);

        return roll switch
        {
            <= 5  => LuckOutcome.DivineFavour,
            <= 25 => LuckOutcome.BlessedTick,
            <= 75 => LuckOutcome.NormalTick,
            <= 95 => LuckOutcome.GodLuckIsLow,
            _     => LuckOutcome.CursedTick,
        };
    }

    public TickOutcome ResolveEvent(Champion champion, WorldEvent worldEvent, StatEffect? interventionEffect, LuckOutcome luckOutcome)
    {
        var rng = new Random();

        // Apply intervention stat boosts temporarily for this tick
        var effectiveStats = ApplyEffect(champion.Stats, interventionEffect);

        // Apply active debuff penalty to effective stats
        effectiveStats = ApplyDebuff(effectiveStats, champion.ActiveDebuff);

        // Parse stat requirements from world event
        var requirements = ParseStatRequirements(worldEvent.StatRequirementsJson);

        // Determine success based on effective stats vs requirements, adjusted by personality
        bool meetsRequirements = MeetsRequirements(effectiveStats, requirements);
        int successThreshold = meetsRequirements ? 80 : 40;
        successThreshold = ApplyPersonalitySuccessModifier(successThreshold, champion.PersonalityTrait, worldEvent);

        bool success = rng.Next(1, 101) <= successThreshold;

        // Calculate base XP and HP outcomes
        int baseXp = success ? rng.Next(20, 51) : rng.Next(5, 21);
        int hpDelta = success ? rng.Next(0, 16) : -rng.Next(5, 21);

        // Cautious trait: -5% damage taken on failure
        if (!success && champion.PersonalityTrait == PersonalityTrait.Cautious)
            hpDelta = (int)(hpDelta * 0.95);

        // Reckless trait: higher variance — amplify both wins and losses
        if (champion.PersonalityTrait == PersonalityTrait.Reckless)
        {
            baseXp = success ? rng.Next(25, 65) : rng.Next(3, 15);
            hpDelta = success ? rng.Next(0, 22) : -rng.Next(8, 28);
        }

        // Apply luck multiplier to XP
        double luckMultiplier = luckOutcome switch
        {
            LuckOutcome.DivineFavour  => 1.0 + rng.NextDouble() * 0.15 + 0.25, // +25 to +40%
            LuckOutcome.BlessedTick   => 1.0 + rng.NextDouble() * 0.15 + 0.10, // +10 to +25%
            LuckOutcome.NormalTick    => 1.0,
            LuckOutcome.GodLuckIsLow  => 1.0 - (rng.NextDouble() * 0.10 + 0.10), // -10 to -20%
            LuckOutcome.CursedTick    => 0.75, // flat -25% on cursed ticks
            _                         => 1.0
        };
        int xpGained = Math.Max(1, (int)(baseXp * luckMultiplier));

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

        // Tick down active debuff
        if (champion.ActiveDebuff != DebuffType.None)
        {
            champion.ActiveDebuffTicksRemaining--;
            if (champion.ActiveDebuffTicksRemaining <= 0)
                champion.ActiveDebuff = DebuffType.None;
        }

        // Apply new debuff from luck
        if (luckOutcome is LuckOutcome.CursedTick or LuckOutcome.GodLuckIsLow)
        {
            var debuff = luckOutcome == LuckOutcome.CursedTick
                ? PickCurseDebuff(rng)
                : DebuffType.StatReduction;
            champion.ActiveDebuff = debuff;
            champion.ActiveDebuffTicksRemaining = luckOutcome == LuckOutcome.CursedTick ? 2 : 1;
        }

        // Track win/loss for patron title
        if (success) champion.CombatWins++;
        champion.TicksSurvivedStreak++;

        // Track consecutive cursed ticks
        if (luckOutcome == LuckOutcome.CursedTick)
            champion.ConsecutiveCursedTicks++;
        else
            champion.ConsecutiveCursedTicks = 0;

        if (luckOutcome == LuckOutcome.DivineFavour)
            champion.DivineFavourCount++;

        // Level up check: XP threshold = Level * 100
        bool leveledUp = false;
        while (champion.XP >= champion.Level * 100)
        {
            champion.XP -= champion.Level * 100;
            champion.Level++;
            champion.MaxHP += 10;
            champion.HP = Math.Min(champion.HP + 10, champion.MaxHP);

            champion.Stats = champion.Stats with
            {
                STR = champion.Stats.STR + 1,
                DEX = champion.Stats.DEX + 1,
                INT = champion.Stats.INT + 1,
                WIS = champion.Stats.WIS + 1,
                VIT = champion.Stats.VIT + 1
            };

            champion.Biome = champion.Level switch
            {
                >= 10 => Biome.Dangerous,
                >= 5  => Biome.Normal,
                _     => champion.Biome
            };

            leveledUp = true;
        }

        string description = success
            ? $"succeeded in the {worldEvent.Name} encounter, gaining {xpGained} XP"
            : $"struggled through {worldEvent.Name}, taking {-hpDelta} damage but earning {xpGained} XP";

        if (luckOutcome == LuckOutcome.DivineFavour)
            description = $"[Divine Favour] {description}";
        else if (luckOutcome == LuckOutcome.CursedTick)
            description = $"[Cursed] {description}";

        return new TickOutcome(champion.Id, xpGained, hpDelta, worldEvent.Name, description, luckOutcome, leveledUp);
    }

    private static int ApplyPersonalitySuccessModifier(int baseThreshold, PersonalityTrait trait, WorldEvent worldEvent)
    {
        // Brave: +5% combat outcome bonus
        if (trait == PersonalityTrait.Brave)
            return baseThreshold + 5;

        // Cautious: slight defensive bonus (treated as better success in exploration)
        if (trait == PersonalityTrait.Cautious && worldEvent.Biome == Biome.Safe)
            return baseThreshold + 5;

        return baseThreshold;
    }

    private static Stats ApplyDebuff(Stats stats, DebuffType debuff) => debuff switch
    {
        DebuffType.StatReduction  => stats with { STR = stats.STR - 5, DEX = stats.DEX - 5 },
        DebuffType.WeakenedStrike => stats with { STR = (int)(stats.STR * 0.9), DEX = (int)(stats.DEX * 0.9) },
        DebuffType.CursedBlood    => stats with { VIT = (int)(stats.VIT * 0.9), WIS = (int)(stats.WIS * 0.9) },
        _                         => stats
    };

    private static DebuffType PickCurseDebuff(Random rng) => rng.Next(3) switch
    {
        0 => DebuffType.StatReduction,
        1 => DebuffType.WeakenedStrike,
        _ => DebuffType.CursedBlood,
    };

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
