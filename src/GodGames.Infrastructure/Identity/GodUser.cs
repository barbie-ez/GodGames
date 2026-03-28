using GodGames.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GodGames.Infrastructure.Identity;

public class GodUser : IdentityUser<Guid>
{
    public PatronTitle PatronTitle { get; set; } = PatronTitle.TheHopeful;
}
