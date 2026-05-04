using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ExampleTemplate.WebApp.Database.EntityFramework.PostgreSql;

/// <summary>
/// Design time factory for the PostgreSql Database Provider
/// </summary>
public class PostgresAppDbContextDesignTimeFactory
    : IDesignTimeDbContextFactory<PostgresAppDbContext>
{
    PostgresAppDbContext IDesignTimeDbContextFactory<PostgresAppDbContext>.CreateDbContext(string[] args)
        => CreateDbContext();

    internal static PostgresAppDbContext CreateDbContext()
        => new(new DbContextOptionsBuilder<PostgresAppDbContext>()
            .UseNpgsql(Environment.GetEnvironmentVariable("Database__ConnectionString"))
            .Options
        );
}
