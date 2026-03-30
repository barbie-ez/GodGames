using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using MediatR;

namespace GodGames.Application.Champions;

public record GetMyChampionQuery(Guid GodId) : IRequest<ChampionDto?>;

public class GetMyChampionHandler(IChampionRepository repo) : IRequestHandler<GetMyChampionQuery, ChampionDto?>
{
    public async Task<ChampionDto?> Handle(GetMyChampionQuery request, CancellationToken ct)
    {
        var champion = await repo.GetByGodIdAsync(request.GodId, ct);
        return champion is null ? null : CreateChampionHandler.ToDto(champion);
    }
}
