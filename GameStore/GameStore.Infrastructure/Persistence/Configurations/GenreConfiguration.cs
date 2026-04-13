using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations;

public class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        //CREATE TABLE GENRES (ID VARCHAR2(120) NOT NULL, NAME VARCHAR2(100) NOT NULL, DESCRIPTION VARCHAR2(500);
        builder.ToTable("PG_Genres");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(500);
        
    }
}