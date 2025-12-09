using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.QueryDatabase;

public class QueryDatabaseContext : DbContext
{
    public QueryDatabaseContext(DbContextOptions<QueryDatabaseContext> options) : base(options)
    {
    }

    public DbSet<Domain.Bank> Banks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Bank>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Country).IsRequired();
        });
    }
}