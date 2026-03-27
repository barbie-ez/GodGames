using GodGames.Application.Models;
using GodGames.Domain.Entities;

namespace GodGames.Application.Interfaces;

public interface IGameEngineService
{
    TickOutcome ResolveEvent(Champion champion, WorldEvent worldEvent, StatEffect? interventionEffect);
}
