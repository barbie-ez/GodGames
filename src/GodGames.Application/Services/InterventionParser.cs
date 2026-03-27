using GodGames.Application.Interfaces;
using GodGames.Application.Models;

namespace GodGames.Application.Services;

public class InterventionParser : IInterventionParser
{
    public StatEffect Parse(string rawCommand)
    {
        var cmd = rawCommand.ToLowerInvariant();

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

        // Default: minor blessing
        return new StatEffect(VIT: 5);
    }

    private static bool Contains(string input, params string[] keywords)
        => keywords.Any(input.Contains);
}
