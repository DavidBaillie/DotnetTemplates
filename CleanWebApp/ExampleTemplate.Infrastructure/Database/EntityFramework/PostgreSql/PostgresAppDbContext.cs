using ExampleTemplate.Infrastructure.Database.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ExampleTemplate.Infrastructure.Database.EntityFramework.PostgreSql;

/// <summary>
/// Defines an AppDbContext that can be customized for accessing a PostgreSql database from 
/// Entity Framework.
/// </summary>
public sealed class PostgresAppDbContext(DbContextOptions<PostgresAppDbContext> options)
    : AppDbContext(options)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
