using GodGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GodGames.Infrastructure.Persistence.Configurations;

public class GodConfiguration : IEntityTypeConfiguration<God>
{
    public void Configure(EntityTypeBuilder<God> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(g => g.Email)
            .IsUnique();

        builder.HasOne(g => g.Champion)
            .WithOne(c => c.God)
            .HasForeignKey<Champion>(c => c.GodId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
