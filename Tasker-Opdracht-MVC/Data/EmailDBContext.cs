using Microsoft.EntityFrameworkCore;
using Tasker_Opdracht_MVC.Data.Entities;

namespace Tasker_Opdracht_MVC.Data;

public class EmailDBContext : DbContext
{
    public DbSet<Email> Forms { get; set; }

    public EmailDBContext(DbContextOptions<EmailDBContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
