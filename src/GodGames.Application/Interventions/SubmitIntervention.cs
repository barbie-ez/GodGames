using System.Text.Json;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Entities;
using MediatR;

namespace GodGames.Application.Interventions;

public record SubmitInterventionCommand(Guid GodId, Guid ChampionId, string RawCommand) : IRequest<InterventionDto>;

public class SubmitInterventionHandler(
    IInterventionRepository repo,
    IInterventionParser parser) : IRequestHandler<SubmitInterventionCommand, InterventionDto>
{
    public async Task<InterventionDto> Handle(SubmitInterventionCommand request, CancellationToken ct)
    {
        var existing = await repo.GetPendingByGodIdAsync(request.GodId, ct);
        if (existing is not null)
            throw new InvalidOperationException("A pending intervention already exists for this tick window.");

        var effect = parser.Parse(request.RawCommand);

        var intervention = new Intervention
        {
            Id = Guid.NewGuid(),
            GodId = request.GodId,
            ChampionId = request.ChampionId,
            RawCommand = request.RawCommand,
            ParsedEffectJson = JsonSerializer.Serialize(effect),
            IsApplied = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await repo.AddAsync(intervention, ct);

        return new InterventionDto(
            intervention.Id,
            intervention.GodId,
            intervention.ChampionId,
            intervention.RawCommand,
            intervention.IsApplied,
            intervention.CreatedAt);
    }
}
