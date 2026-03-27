using GodGames.Application.Models;

namespace GodGames.Application.Interfaces;

public interface IInterventionParser
{
    StatEffect Parse(string rawCommand);
}
