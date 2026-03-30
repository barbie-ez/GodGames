namespace GodGames.Domain.Enums;

public enum DebuffType
{
    None = 0,
    StatReduction = 1,  // -5 to a random stat
    WeakenedStrike = 2, // -10% to STR and DEX
    CursedBlood = 3,    // -10% to VIT and WIS
}
