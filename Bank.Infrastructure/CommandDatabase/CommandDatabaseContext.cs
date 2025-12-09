using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.CommandDatabase;

public class CommandDatabaseContext : DbContext
{
    public CommandDatabaseContext(DbContextOptions<CommandDatabaseContext> options) : base(options)
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