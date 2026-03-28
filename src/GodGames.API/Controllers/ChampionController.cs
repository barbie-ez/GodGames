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
public class ChampionController(
    IMediator mediator,
    INarrativeRepository narratives,
    IChampionRepository champions,
    IGodRepository gods) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateChampionRequest request, CancellationToken ct)
    {
        var godId = GetGodId();
        var champion = await mediator.Send(
            new CreateChampionCommand(godId, request.Name, request.Class, request.PersonalityTrait), ct);
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
    public async Task<IActionResult> GetNarratives([FromQuery] int count = 10, CancellationToken ct = default)
    {
        var godId = GetGodId();
        var champion = await mediator.Send(new GetMyChampionQuery(godId), ct);
        if (champion is null) return NotFound();

        var clamped = Math.Clamp(count, 1, 50);
        var entries = await narratives.GetLastNByChampionIdAsync(champion.Id, clamped, ct);
        var dtos = entries.Select(e => new NarrativeEntryDto(e.Id, e.ChampionId, e.TickNumber, e.StoryText, e.CreatedAt));
        return Ok(dtos);
    }

    [HttpGet("leaderboard")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeaderboard(CancellationToken ct)
    {
        var all = await champions.GetAllActiveAsync(ct);
        var ranked = new List<LeaderboardEntryDto>();
        int rank = 0;
        foreach (var c in all.OrderByDescending(c => c.Level).ThenByDescending(c => c.XP))
        {
            rank++;
            var patronTitle = await gods.GetPatronTitleAsync(c.GodId, ct);
            ranked.Add(new LeaderboardEntryDto(rank, c.Name, c.Class, c.PersonalityTrait, c.Level, c.XP, c.Biome, patronTitle));
        }
        return Ok(ranked);
    }

    private Guid GetGodId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record CreateChampionRequest(
    string Name,
    ChampionClass Class,
    PersonalityTrait PersonalityTrait = PersonalityTrait.Brave);

public record LeaderboardEntryDto(
    int Rank,
    string Name,
    ChampionClass Class,
    PersonalityTrait PersonalityTrait,
    int Level,
    int XP,
    Biome Biome,
    PatronTitle PatronTitle);
