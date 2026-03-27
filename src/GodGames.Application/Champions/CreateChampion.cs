using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using GodGames.Domain.Entities;
using GodGames.Domain.Enums;
using GodGames.Domain.ValueObjects;
using MediatR;

namespace GodGames.Application.Champions;

public record CreateChampionCommand(Guid GodId, string Name, ChampionClass Class) : IRequest<ChampionDto>;

public class CreateChampionHandler(IChampionRepository repo) : IRequestHandler<CreateChampionCommand, ChampionDto>
{
    public async Task<ChampionDto> Handle(CreateChampionCommand request, CancellationToken ct)
    {
        var existing = await repo.GetByGodIdAsync(request.GodId, ct);
        if (existing is not null)
            throw new InvalidOperationException("This god already has a champion.");

        var champion = new Champion
        {
            Id = Guid.NewGuid(),
            GodId = request.GodId,
            Name = request.Name,
            Class = request.Class,
            Stats = new Stats(10, 10, 10, 10, 10),
            HP = 100,
            MaxHP = 100,
            Level = 1,
            XP = 0,
            Biome = Biome.Safe,
            CreatedAt = DateTimeOffset.UtcNow,
            LastTickAt = DateTimeOffset.UtcNow
        };

        await repo.AddAsync(champion, ct);
        return ToDto(champion);
    }

    internal static ChampionDto ToDto(Champion c) => new(
        c.Id, c.GodId, c.Name, c.Class,
        c.Stats.STR, c.Stats.DEX, c.Stats.INT, c.Stats.WIS, c.Stats.VIT,
        c.HP, c.MaxHP, c.Level, c.XP, c.PowerUpSlot, c.Biome,
        c.CreatedAt, c.LastTickAt);
}
