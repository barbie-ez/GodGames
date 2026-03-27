using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class NarrativeEntryConfiguration : IEntityTypeConfiguration<NarrativeEntry>
{
    public void Configure(EntityTypeBuilder<NarrativeEntry> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.StoryText)
            .IsRequired();

        builder.HasOne(n => n.Champion)
            .WithMany(c => c.NarrativeEntries)
            .HasForeignKey(n => n.ChampionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => new { n.ChampionId, n.TickNumber });
    }
}
