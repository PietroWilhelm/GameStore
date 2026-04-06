using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations;

/// <summary>
/// Configuração da EF para hierarquia content (Game, Studio)
/// </summary>
public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("PG_Content");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(c => c.LaunchDate)
            .IsRequired();

        builder.HasDiscriminator<string>("Type")
            .HasValue<Game>("Game")
            .HasValue<Studio>("Studio");
           

        builder.Property("Type")
            .HasMaxLength(20);

        // N:N com Genre
       builder.HasMany(c => c.Genres)
           .WithMany(g => g.Contents)
           .UsingEntity(j => j.ToTable("ContentGenres"));

        // 1:N Ratings
        //builder.HasMany(c => c.Ratings)
        //    .WithOne()
        //    .HasForeignKey(r => r.ContentId)
        //    .OnDelete(DeleteBehavior.Cascade);
        
        
        
        
    }
}