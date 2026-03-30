using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Enums;

namespace GodGames.Application.Services;

public class InterventionParser : IInterventionParser
{
    public StatEffect Parse(string rawCommand)
    {
        var cmd = rawCommand.ToLowerInvariant().Trim();

        // "move to <region name>" — converts region name to a slug ID
        if (cmd.StartsWith("move to "))
        {
            var regionName = cmd["move to ".Length..].Trim();
            var regionId = ToRegionId(regionName);
            return new StatEffect(MoveToRegionId: regionId);
        }

        if (Contains(cmd, "fire resistance", "heat"))
            return new StatEffect(VIT: 10, DurationTicks: 3);

        if (Contains(cmd, "blessed blade", "holy sword"))
            return new StatEffect(STR: 15, DurationTicks: 3);

        if (Contains(cmd, "divine shield", "protect"))
            return new StatEffect(HP: 20);

        if (Contains(cmd, "swift feet", "speed", "haste"))
            return new StatEffect(DEX: 15, DurationTicks: 3);

        if (Contains(cmd, "arcane mind", "wisdom"))
            return new StatEffect(WIS: 15, DurationTicks: 3);

        if (Contains(cmd, "inner eye", "insight"))
            return new StatEffect(INT: 15, DurationTicks: 3);

        if (Contains(cmd, "battle rage", "berserk"))
            return new StatEffect(STR: 20, WIS: -5, DurationTicks: 3);

        if (Contains(cmd, "stealth", "shadow"))
            return new StatEffect(DEX: 10, DurationTicks: 3);

        if (Contains(cmd, "fortify", "endure"))
            return new StatEffect(VIT: 10, HP: 5);

        if (Contains(cmd, "smite"))
            return new StatEffect(STR: 10, INT: 5, DurationTicks: 3);

        if (Contains(cmd, "rejuvenate", "heal", "mend"))
            return new StatEffect(HP: 30);

        // Default: minor blessing — Cunning trait results in a slightly better default
        return new StatEffect(VIT: 5);
    }

    /// Applies a bonus multiplier for Cunning champions (better intervention results).
    public StatEffect ParseWithPersonality(string rawCommand, PersonalityTrait trait)
    {
        var effect = Parse(rawCommand);

        if (trait != PersonalityTrait.Cunning) return effect;

        // Cunning: +15% to all stat values (rounded up), does not affect HP or MoveToRegionId
        return effect with
        {
            STR = ApplyCunningBonus(effect.STR),
            DEX = ApplyCunningBonus(effect.DEX),
            INT = ApplyCunningBonus(effect.INT),
            WIS = ApplyCunningBonus(effect.WIS),
            VIT = ApplyCunningBonus(effect.VIT),
        };
    }

    private static int ApplyCunningBonus(int value)
        => value > 0 ? (int)Math.Ceiling(value * 1.15) : value;

    private static string ToRegionId(string name)
        => name.Replace(" ", "-").Replace("'", "").Replace("\"", "");

    private static bool Contains(string input, params string[] keywords)
        => keywords.Any(input.Contains);
}
