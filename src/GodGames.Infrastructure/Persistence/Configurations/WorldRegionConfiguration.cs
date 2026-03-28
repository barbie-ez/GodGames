using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class WorldRegionConfiguration : IEntityTypeConfiguration<WorldRegion>
{
    public void Configure(EntityTypeBuilder<WorldRegion> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasMaxLength(100);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Biome)
            .HasConversion<string>();

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.ActiveEventTypes)
            .HasMaxLength(200);
    }
}
