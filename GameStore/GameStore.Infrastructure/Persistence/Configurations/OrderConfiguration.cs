using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("PG_Orders");
        
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalValue)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(o => o.OrderDate).IsRequired();
  
        
        // 1:N (Pedido -> Cliente)
      builder.HasOne(o => o.Customer)
          .WithMany()
          .HasForeignKey(o => o.CustomerId)
          .OnDelete(DeleteBehavior.Restrict);
}
    
}