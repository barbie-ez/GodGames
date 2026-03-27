using System.Security.Claims;
using GodGames.Application.Champions;
using GodGames.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodGames.API.Controllers;

[ApiController]
[Route("api/champions")]
[Authorize]
public class ChampionController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChampionRequest request, CancellationToken ct)
    {
        var godId = GetGodId();
        var champion = await mediator.Send(new CreateChampionCommand(godId, request.Name, request.Class), ct);
        return CreatedAtAction(nameof(GetMine), champion);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        var godId = GetGodId();
        var champion = await mediator.Send(new GetMyChampionQuery(godId), ct);
        return champion is null ? NotFound() : Ok(champion);
    }

    private Guid GetGodId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record CreateChampionRequest(string Name, ChampionClass Class);
