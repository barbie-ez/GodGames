using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class ChampionConfiguration : IEntityTypeConfiguration<Champion>
{
    public void Configure(EntityTypeBuilder<Champion> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Class)
            .HasConversion<string>();

        builder.Property(c => c.Biome)
            .HasConversion<string>();

        builder.OwnsOne(c => c.Stats, stats =>
        {
            stats.Property(s => s.STR).HasColumnName("stat_str");
            stats.Property(s => s.DEX).HasColumnName("stat_dex");
            stats.Property(s => s.INT).HasColumnName("stat_int");
            stats.Property(s => s.WIS).HasColumnName("stat_wis");
            stats.Property(s => s.VIT).HasColumnName("stat_vit");
        });
    }
}
