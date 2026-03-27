using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GodGames.API.Hubs;

[Authorize]
public class GameHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var godId = Context.UserIdentifier;
        if (godId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, godId);

        await base.OnConnectedAsync();
    }
}
