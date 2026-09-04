using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuração da EF para a entidade base Content.
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


        // N:N com Genre
        builder.HasMany(c => c.Genres)
            .WithMany(g => g.Contents)
            .UsingEntity(j => j.ToTable("ContentGenres"));

        // 1:N Ratings
        //builder.HasMany(c => c.Ratings)
        //    .WithOne()
        //    .HasForeignKey(r => r.ContentId)
        //    .OnDelete(DeleteBehavior.Cascade);

        // TPH: todas as entidades derivadas de Content ficam na mesma tabela (PG_Content),
        // diferenciadas pela coluna discriminadora "Type".
        builder.HasDiscriminator<string>("Type")
            .HasValue<Game>("Game");

        builder.Property<string>("Type")
            .HasMaxLength(32)
            .IsRequired();
    }
}
