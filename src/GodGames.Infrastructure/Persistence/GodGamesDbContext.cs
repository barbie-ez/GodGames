using GodGames.Domain.Entities;
using GodGames.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GodGames.Infrastructure.Persistence;

public class GodGamesDbContext(DbContextOptions<GodGamesDbContext> options)
    : IdentityDbContext<GodUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Champion> Champions => Set<Champion>();
    public DbSet<WorldEvent> WorldEvents => Set<WorldEvent>();
    public DbSet<Intervention> Interventions => Set<Intervention>();
    public DbSet<NarrativeEntry> NarrativeEntries => Set<NarrativeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
