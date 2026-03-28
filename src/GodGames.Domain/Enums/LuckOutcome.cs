namespace GodGames.Domain.Enums;

public enum LuckOutcome
{
    DivineFavour = 0,  // 5%  — +25 to +40% bonus to all stat gains
    BlessedTick = 1,   // 20% — +10 to +25% bonus to primary stat gain
    NormalTick = 2,    // 50% — base stat gains, no modifier
    GodLuckIsLow = 3,  // 20% — -10 to -20% to stat gains, minor debuff
    CursedTick = 4,    // 5%  — stat debuff applied for 2 ticks
}
