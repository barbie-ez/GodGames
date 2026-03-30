using GodGames.Application.Models;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;

namespace GodGames.Application.Interfaces;

public interface IGameEngineService
{
    LuckOutcome LuckRoll(Guid championId, int tickNumber);
    TickOutcome ResolveEvent(Champion champion, WorldEvent worldEvent, StatEffect? interventionEffect, LuckOutcome luckOutcome);
}
