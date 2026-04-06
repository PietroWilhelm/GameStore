using GameStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Persistence;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> GameStoreOptions) : base(GameStoreOptions)
    {
        
    }
    
    public DbSet<Game> Games { get; set; }

    public DbSet<Genre> Genres { get; set; }
    
    public DbSet<Customer> customers { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<Studio> Studios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameStoreContext).Assembly);
    }
}
