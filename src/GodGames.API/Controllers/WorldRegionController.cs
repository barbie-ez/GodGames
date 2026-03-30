using GodGames.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GodGames.API.Controllers;

[ApiController]
[Route("api/regions")]
public class WorldRegionController(IWorldRegionRepository regions) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var all = await regions.GetAllAsync(ct);
        var dtos = all.Select(r => new WorldRegionDto(
            r.Id, r.Name, r.Biome.ToString(), r.DifficultyRating,
            r.MinLevelRequired, r.MapX, r.MapY,
            r.Description, r.ActiveEventTypes));
        return Ok(dtos);
    }
}

public record WorldRegionDto(
    string Id,
    string Name,
    string Biome,
    int DifficultyRating,
    int MinLevelRequired,
    int MapX,
    int MapY,
    string Description,
    string ActiveEventTypes);
