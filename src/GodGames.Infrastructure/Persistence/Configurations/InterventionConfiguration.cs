using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.RawCommand)
            .IsRequired();

        builder.Property(i => i.ParsedEffectJson)
            .HasColumnType("text");

        builder.HasOne(i => i.Champion)
            .WithMany(c => c.Interventions)
            .HasForeignKey(i => i.ChampionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => new { i.GodId, i.IsApplied });
    }
}
