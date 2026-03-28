using GodGames.Application.Interfaces;
using GodGames.Domain.Enums;
using GodGames.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GodGames.Infrastructure.Repositories;

public class GodRepository(UserManager<GodUser> userManager) : IGodRepository
{
    public async Task UpdatePatronTitleAsync(Guid godId, PatronTitle title, CancellationToken ct = default)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == godId, ct);
        if (user is null) return;
        if (user.PatronTitle == title) return;

        user.PatronTitle = title;
        await userManager.UpdateAsync(user);
    }

    public async Task<PatronTitle> GetPatronTitleAsync(Guid godId, CancellationToken ct = default)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == godId, ct);
        return user?.PatronTitle ?? PatronTitle.TheHopeful;
    }
}
