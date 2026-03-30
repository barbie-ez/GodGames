using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GodGames.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GodGamesDbContext>
{
    public GodGamesDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("GODGAMES_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=godgames;Username=godgames;Password=godgames_dev";

        var options = new DbContextOptionsBuilder<GodGamesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new GodGamesDbContext(options);
    }
}
