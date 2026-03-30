using System.Text.Json;
using GodGames.API.Hubs;
using GodGames.Infrastructure.Notifications;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace GodGames.API.Services;

public class ChampionUpdateListener(
    IConnectionMultiplexer redis,
    IHubContext<GameHub> hub,
    ILogger<ChampionUpdateListener> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sub = redis.GetSubscriber();

        await sub.SubscribeAsync(RedisChannel.Literal(RedisTickNotifier.Channel), async (_, message) =>
        {
            if (message.IsNullOrEmpty) return;
            try
            {
                var payload = JsonSerializer.Deserialize<ChampionUpdateMessage>(message!);
                if (payload is null) return;

                await hub.Clients
                    .Group(payload.GodId.ToString())
                    .SendAsync("ChampionUpdated", payload.Champion);

                logger.LogDebug("Pushed ChampionUpdated to god {GodId}", payload.GodId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to push ChampionUpdated");
            }
        });

        await Task.Delay(Timeout.Infinite, stoppingToken);

        await sub.UnsubscribeAsync(RedisChannel.Literal(RedisTickNotifier.Channel));
    }
}
