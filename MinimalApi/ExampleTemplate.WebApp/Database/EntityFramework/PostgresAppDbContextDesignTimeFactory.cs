using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SukkotStore.WebApp.Database.EntityFramework;

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
