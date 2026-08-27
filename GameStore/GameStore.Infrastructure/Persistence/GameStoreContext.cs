using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Persistence;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> GameStoreOptions) : base(GameStoreOptions)
    {
        
    }
    
    public DbSet<Content> Contents { get; set; }
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }
    
    public DbSet<Customer> customers { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<Studio> Studios { get; set; }
    
    public DbSet<CustomerConfiguration> CustomerConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map CLR bool Active to NUMBER(1) in Oracle to avoid PL/SQL type mismatches when triggers exist on tables.
        // Convert: true -> 1, false -> 0
        var boolToIntConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<bool, int>(
            v => v ? 1 : 0,
            v => v == 1);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var activeProperty = entityType.FindProperty("Active");
            if (activeProperty != null && activeProperty.ClrType == typeof(bool))
            {
                activeProperty.SetValueConverter(boolToIntConverter);
                activeProperty.SetColumnType("NUMBER(1)");
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameStoreContext).Assembly);
    }
}
