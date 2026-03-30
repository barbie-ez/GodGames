using GodGames.Domain.Entities;
using GodGames.Domain.Enums;

namespace GodGames.Application.Services;

public static class PatronTitleService
{
    /// Evaluates the patron title a god should hold based on their champion's stats.
    public static PatronTitle EvaluateTitle(Champion champion) => champion switch
    {
        { DivineFavourCount: >= 5 }           => PatronTitle.TheFortunate,
        { ConsecutiveCursedTicks: >= 3 }       => PatronTitle.TheCursed,
        { CombatWins: >= 10 }                  => PatronTitle.TheMerciless,
        { TicksSurvivedStreak: >= 20 }         => PatronTitle.TheProtector,
        _                                      => PatronTitle.TheHopeful,
    };
}
