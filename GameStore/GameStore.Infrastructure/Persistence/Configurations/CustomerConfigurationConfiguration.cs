using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameStore.Infrastructure.Configurations;

/// <summary>
/// Configuração EF para a entidade UserConfiguration.
/// </summary>
public class CustomerConfigurationConfiguration: IEntityTypeConfiguration<GameStore.Domain.Entities.CustomerConfiguration>
{
    public void Configure(EntityTypeBuilder<GameStore.Domain.Entities.CustomerConfiguration> builder)
    {
        builder.ToTable("PG_UserConfigurations");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.CustomerId)
            .IsRequired();

        builder.HasIndex(uc => uc.CustomerId)
            .IsUnique();

        builder.Property(uc => uc.Theme)
            .HasMaxLength(50)
            .HasDefaultValue("Dark");

        builder.Property(uc => uc.EnableNotifications)
            .HasDefaultValue(false);
    }
}