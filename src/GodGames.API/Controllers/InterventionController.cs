using System.Security.Claims;
using GodGames.Application.Interventions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodGames.API.Controllers;

[ApiController]
[Route("api/interventions")]
[Authorize]
public class InterventionController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitInterventionRequest request, CancellationToken ct)
    {
        var godId = GetGodId();
        var intervention = await mediator.Send(
            new SubmitInterventionCommand(godId, request.ChampionId, request.RawCommand), ct);
        return Created($"/api/interventions/{intervention.Id}", intervention);
    }

    private Guid GetGodId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record SubmitInterventionRequest(Guid ChampionId, string RawCommand);
