using System.Security.Claims;
using GodGames.Application.Champions;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodGames.API.Controllers;

[ApiController]
[Route("api/champions")]
[Authorize]
public class ChampionController(IMediator mediator, INarrativeRepository narratives) : ControllerBase
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

    [HttpGet("me/narratives")]
    public async Task<IActionResult> GetNarratives(CancellationToken ct)
    {
        var godId = GetGodId();
        var champion = await mediator.Send(new GetMyChampionQuery(godId), ct);
        if (champion is null) return NotFound();

        var entries = await narratives.GetLastNByChampionIdAsync(champion.Id, 10, ct);
        var dtos = entries.Select(e => new NarrativeEntryDto(e.Id, e.ChampionId, e.TickNumber, e.StoryText, e.CreatedAt));
        return Ok(dtos);
    }

    private Guid GetGodId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record CreateChampionRequest(string Name, ChampionClass Class);
