using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations;

public class CustomerConfiguration: IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("PG_Customers");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(c => c.Cpf)
            .HasMaxLength(11)
            .IsRequired();
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Password)
            .HasMaxLength(500)
            .IsRequired();

        // BirthDate -> SetBirhDate (propriedade privada)
        builder.Property<DateOnly>("SetBirhDate")
            .HasColumnName("BirthDate")
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        // Salt (backing field da auto-property)
        builder.Property("Salt")
            .HasColumnName("Salt")
            .HasMaxLength(100)
            .IsRequired();
        
        //1..N -> Customer possui muitos Pedidos (Order)
        builder.HasMany<Order>()
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        
        //1..1 -> Customer possui uma CustomerConfiguration
        builder.HasOne(u => u.Configuration)
            .WithOne()
            .HasForeignKey<GameStore.Domain.Entities.CustomerConfiguration>(uc => uc.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}