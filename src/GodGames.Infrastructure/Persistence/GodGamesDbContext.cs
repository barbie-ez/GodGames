using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GodGames.Infrastructure.Persistence;

public class GodGamesDbContext(DbContextOptions<GodGamesDbContext> options) : DbContext(options)
{
    public DbSet<God> Gods => Set<God>();
    public DbSet<Champion> Champions => Set<Champion>();
    public DbSet<WorldEvent> WorldEvents => Set<WorldEvent>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<NarrativeEntry> NarrativeEntries => Set<NarrativeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}
