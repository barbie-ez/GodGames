using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class WorldEventConfiguration : IEntityTypeConfiguration<WorldEvent>
{
    public void Configure(EntityTypeBuilder<WorldEvent> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Description)
            .IsRequired();

        builder.Property(w => w.Biome)
            .HasConversion<string>();

        builder.Property(w => w.StatRequirementsJson)
            .HasColumnType("text");

        builder.Property(w => w.OutcomeModifiersJson)
            .HasColumnType("text");
    }
}
