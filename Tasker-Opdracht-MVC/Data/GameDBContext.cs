using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Data;

public class GameDBContext : DbContext
{
    public DbSet<Game> Games { get; set; }

    public GameDBContext(DbContextOptions<GameDBContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SeedData();
    }
}
