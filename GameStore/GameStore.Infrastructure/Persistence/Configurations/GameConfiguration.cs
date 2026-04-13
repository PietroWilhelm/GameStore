using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("PG_Games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(1000);
        
        builder.Property(g => g.LaunchDate)
            .IsRequired();

        // N..N Com generos
        builder.HasMany(g => g.Genres)
            .WithMany()
            .UsingEntity(j => j.ToTable("OrderGames"));
        
        //  1:N com Studio
        builder.HasOne(g => g.Studio)
            .WithMany(s => s.Games)
            .HasForeignKey(g => g.StudioId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
    }
}