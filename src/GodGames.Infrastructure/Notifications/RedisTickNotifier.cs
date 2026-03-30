using System.Text.Json;
using GodGames.Application.Interfaces;
using GodGames.Application.Models;
using StackExchange.Redis;

namespace GodGames.Infrastructure.Notifications;

public class RedisTickNotifier(IConnectionMultiplexer redis) : ITickNotifier
{
    public const string Channel = "champion-updated";

    public async Task NotifyChampionUpdatedAsync(Guid godId, ChampionDto champion, CancellationToken ct = default)
    {
        var sub = redis.GetSubscriber();
        var payload = JsonSerializer.Serialize(new ChampionUpdateMessage(godId, champion));
        await sub.PublishAsync(RedisChannel.Literal(Channel), payload);
    }
}

public record ChampionUpdateMessage(Guid GodId, ChampionDto Champion);
