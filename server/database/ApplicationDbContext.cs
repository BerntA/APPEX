using database.Entities;
using Microsoft.EntityFrameworkCore;

namespace database;

public class ApplicationDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .HasIndex(p => p.OrganizationNumber)
            .IsUnique();
    }
}